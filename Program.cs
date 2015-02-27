using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace ConsoleApplication1
{
    class Program
    {
        public static int systemtime = 0;
        public SortedList<int, Task> queue;
        public static List<Task> tasklist;
        public double newvoltage;
        public double avevoltage;
        public int starttime;
        public int finishedtime;
        public int newesttask;
        
        public Program()
        {
            tasklist = new List<Task>();
            queue = new SortedList<int, Task>();

            //maximum voltage is 1
            newvoltage = 10;
            starttime = 0;
            finishedtime = 0;
            newesttask = 0;
            avevoltage = 0;
        }

        public class Task
        {
            public int arrival;
            public int comptime;
            public int deadline;
            public int tasknum;
            public int visited;
            public int accepted;
            public Task()
            {
                arrival = -1;
                comptime = -1;
                deadline = -1;
                tasknum = -1;
                visited = 0;
                accepted = 0;
            }
        }

        //public void PrioQueue(Task newtask)
        //{
        //    //tasks added to the queue based on their deadlines, EDF
        //    queue.Add(newtask.deadline, newtask);

        //}

        //public void scheduler(int time1, int time2)
        //{

        //    if (queue.Count != 0)
        //    {
        //        systemtime = queue.Values[0].arrival;
        //    }
        //    while (queue.Count != 0)
        //    {
        //        systemtime++;
        //        queue.Values[0].comptime--;
        //        //task has been completed
        //        if (queue.Values[0].comptime == 0)
        //        {
        //            Console.WriteLine("task " + queue.Values[0].tasknum + " is finished at time " + systemtime);
        //            queue.RemoveAt(0);
        //        }
        //    }
        //}

        //this function returns the newesr arrived task in the set
        public int MaxinList(List<Task> tasklist, int systemtime)
        {
            int maxindex = 0;
            for (int i = 0; i < tasklist.Count; i++)
            {
                if (tasklist[i].arrival > tasklist[maxindex].arrival && tasklist[i].arrival <= systemtime)
                {
                    maxindex = i;
                }
            }
            return maxindex;
        }
        //this function calculates the new voltage for system when either a new accepted task arrives or when a task
        //finishes executing
        public double CaclulateVoltage(List<Task> tasklist)
        {
            if (tasklist.Count == 0)
                return 1;
            //we decrease the voltaeg to see if the scheduling is possible under EDF
            double tempvoltage2 = 10;
            double tempvoltage = 10;
            int lasttask = MaxinList(tasklist, systemtime);
            int Rcomptimes = 0;
            int maxdeadline = -1;
            
            //calculate the  execution time based on maximum to compare the results correctly for the tasks in the system, last tasks execution time is already based on maximum
            if(lasttask == 0)
            {
                if (((tasklist[lasttask].comptime * newvoltage) % 10) == 0)
                    tasklist[lasttask].comptime = (int)((tasklist[lasttask].comptime * newvoltage) / 10);
                else
                    tasklist[lasttask].comptime = (int)((tasklist[lasttask].comptime * newvoltage) / 10)+1;
            }
            for (int i = 0; i < lasttask; i++)
            {
                if(((tasklist[i].comptime * newvoltage) % 10) == 0)
                    tasklist[i].comptime = (int)((tasklist[i].comptime * newvoltage) / 10);
                else
                    tasklist[i].comptime = (int)((tasklist[i].comptime * newvoltage) / 10) + 1;
            }
            while (tempvoltage2 > 0)
            {
                tempvoltage2 -= 1;
                Rcomptimes = 0;
                maxdeadline = 0;
                for (int i = 0; i <= lasttask; i++)
                {
                    Rcomptimes += (int)(Math.Ceiling((tasklist[i].comptime * 10) / tempvoltage2));
                    if (tasklist[i].deadline > maxdeadline)
                        maxdeadline = tasklist[i].deadline;
                }
                //this is remaining computation time
                if (Rcomptimes + systemtime <= maxdeadline)
                {
                    tempvoltage = tempvoltage2;
                }
                else
                    break;
            }
            newvoltage = tempvoltage;
            for (int i = 0; i <= lasttask; i++)
            {
                if (((tasklist[i].comptime * 10)%newvoltage == 0))
                {
                    tasklist[i].comptime = (int)((tasklist[i].comptime * 10) / newvoltage);
                }
                else
                    tasklist[i].comptime = (int)((tasklist[i].comptime * 10) /newvoltage ) + 1;
            }
            return newvoltage;
        }
        public Task AcceptanceTest(List<Task> tasklist, int indexOflasttask)
        {

            double sum = 0;
            double Uj = 0;
            double max = 0;
            int m = indexOflasttask+1;
            double totalutil = 0;
            Task maxutiltask = new Task();
            List<Task> templist = new List<Task>();
            //create a copy of the list with the tasks (up to now) inside
            for (int i = 0; i < m; i++ )
            {
                Task temptask = new Task();
                temptask.accepted = tasklist[i].accepted;
                temptask.arrival = tasklist[i].arrival;
                temptask.comptime = tasklist[i].comptime;
                temptask.deadline = tasklist[i].deadline;
                temptask.tasknum = tasklist[i].tasknum;
                temptask.visited = tasklist[i].visited;
                templist.Add(temptask);
            }
            //here we need to change the base of voltage to add the remaining computation times to see if the new task is accepted or not
            for (int i = 0; i < indexOflasttask; i++)
            {
                if (((templist[i].comptime * newvoltage) % 10 == 0))
                {
                    templist[i].comptime = (int)((templist[i].comptime * newvoltage) / 10);
                }
                else
                    templist[i].comptime = (int)((templist[i].comptime * newvoltage) / 10) + 1;
            }
            for (int j = 0; j < m; j++)
            {
                sum = 0;
                for (int i = 0; i <= j; i++)
                {
                    sum += templist[i].comptime;
                }
                Uj = sum / (templist[j].deadline - systemtime);
                if (Uj >= max)
                {
                    maxutiltask = templist[j];
                    max = Uj;
                }
            }
            totalutil = max;
            if (totalutil > 1)
                return null;
            else
            {
                starttime = systemtime;
                finishedtime = tasklist[indexOflasttask].deadline;
                tasklist[indexOflasttask].accepted = 1;
                return tasklist[indexOflasttask];
            }
        }

        //scheduler gets the starting time and ending time of the scheduling with certain volate
        // schedule is EDF
        public void schedule()
        {
            int ED = 1000;
            int scheduledtask = 0;
            int i;
            int idel = 0;
            //set the current time of the system
            if (tasklist.Count != 0)
            {
                systemtime = tasklist.ElementAt(0).arrival;
            }
            while (tasklist.Count != 0)
            {
                

                for (i = 0; i < tasklist.Count; i++)
                {
                    if (tasklist[i].arrival <= systemtime)
                    {
                        //see if the task set is schedulable with the newest arrived task or not, accepted tasks don't need another test
                        if ( tasklist[i].accepted == 0 && AcceptanceTest(tasklist, i) != null )
                        {
                            //cpu is going to execute a task
                            idel = 0;
                            
                            //calculate voltage if a new task arrives
                            newesttask = MaxinList(tasklist, systemtime);
                            if (i == newesttask && tasklist[i].visited == 0 /*&& systemtime < finishedtime*/)
                            {
                                tasklist[i].visited = 1;
                                newvoltage = CaclulateVoltage(tasklist);
                            }
                            //EDF scheduling
                            if (tasklist[i].deadline < ED)
                            {
                                //earliest deadline
                                ED = tasklist[i].deadline;
                                scheduledtask = i;
                            }
                        }
                        else if (tasklist[i].accepted == 1)
                        {

                        }
                        else
                        {
                            //we need to remove the task from task set and say it is not going to be schedulable
                            Console.WriteLine("task " + tasklist[i].tasknum + " has been deleted since tasks set will not be schedulable with the current voltage!");
                            tasklist.RemoveAt(i);
                        }
                    }
                    //it means that we don't have any waiting task in the queue so we just increase the timer
                    else if (tasklist[0].arrival > systemtime)
                    {
                        //cpu is going to be idel for a few time slices
                        idel = 1;
                        systemtime++;
                    }
                    else
                        break;
                }
                if (idel == 0)
                {
                    tasklist[scheduledtask].comptime--;
                    Console.WriteLine("task " + tasklist[scheduledtask].tasknum + " starts running at time " + systemtime + " with voltage " + (newvoltage / 10));
                    systemtime++;
                    
                    // if the computation time is zero then the task is removed from the queue
                    if (tasklist[scheduledtask].comptime == 0)
                    {
                        ED = 1000;
                        tasklist.RemoveAt(scheduledtask);
                        //upon finishing a task from the list, we need to calculate voltage again
                        //during the interval calculated using acceptance test, we use the same voltage
                        if (tasklist.Count == 0)
                            return;
                        newvoltage = CaclulateVoltage(tasklist);
                        //rescheduling again
                        for(int k = 0 ; k <tasklist.Count ;k++)
                        {
                            if (tasklist[k].arrival <= systemtime)
                            { 
                                if (tasklist[k].deadline < ED)
                                {
                                    //earliest deadline
                                    ED = tasklist[k].deadline;
                                    scheduledtask = k;
                                }
                            }
                        }
                    }
                   
                    
                }
            }
        }
        public static List<int> ExtractInts(string input)
        {
            return Regex.Matches(input, @"\d+")
               .Cast<Match>()
               .Select(x => Convert.ToInt32(x.Value))
               .ToList();
        }
        public void ReadInput(string infile)
        {
            
            string[] lines = System.IO.File.ReadAllLines(infile);
            //if (lines == null)
            //{
            //    Console.WriteLine("wrong input name");
            //    Environment.Exit(0);
            //}
            Console.WriteLine(lines[0]);
            for (int i = 1; i < lines.Length; i++)
            {
                Task mytask = new Task();
                List<int> result = ExtractInts(lines[i]);
                mytask.tasknum = result[0];
                mytask.arrival = result[1];
                mytask.comptime = result[2];
                mytask.deadline = result[3];
                //Console.WriteLine("num" + mytask.tasknum + " arr" + mytask.arrival + " c"
                //    + mytask.comptime + " d" + mytask.deadline);
                tasklist.Add(mytask);
            }
        }
        static void Main(string[] args)
        {
            // read file from  input line by line
            Program obj = new Program();
            Console.WriteLine("enter the input name (with .txt)");
            string input = Console.ReadLine();
            obj.ReadInput(input);
            obj.schedule();
        }
    }
}
