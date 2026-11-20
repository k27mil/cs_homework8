using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_homework8
{
    public enum ProjectStatus
    {
        Проект,
        Исполнение,
        Закрыт
    }

    public enum WorkTaskStatus
    {
        Назначена,
        ВРаботе,
        НаПроверке,
        Выполнена
    }

    public class Person
    {
        public string Name { get; }

        public Person(string name)
        {
            Name = name;
        }
    }

    public class Report
    {
        public string Text { get; }
        public DateTime CompletionDate { get; }
        public Person Assignee { get; }

        public Report(string text, DateTime completionDate, Person assignee)
        {
            Text = text;
            CompletionDate = completionDate;
            Assignee = assignee;
        }
    }

    public class WorkTask
    {
        public string Description { get; }
        public DateTime Deadline { get; }
        public Person Initiator { get; }
        public Person Assignee { get; private set; }
        public WorkTaskStatus Status { get; private set; }
        public Report Report { get; private set; }

        public WorkTask(string description, DateTime deadline, Person initiator, Person assignee)
        {
            Description = description;
            Deadline = deadline;
            Initiator = initiator;
            Assignee = assignee;
            Status = WorkTaskStatus.Назначена;
        }

        public bool TakeInWork(Person person)
        {
            if (Assignee != person || Status != WorkTaskStatus.Назначена)
            {
                return false;
            }

            Status = WorkTaskStatus.ВРаботе;
            return true;
        }

        public bool Delegate(Person fromPerson, Person toPerson)
        {
            if (Assignee != fromPerson || Status != WorkTaskStatus.Назначена)
            {
                return false;
            }

            Assignee = toPerson;
            Status = WorkTaskStatus.Назначена;
            return true;
        }

        public bool Reject(Person person)
        {
            if (Assignee != person || Status != WorkTaskStatus.Назначена)
            {
                return false;
            }

            Assignee = null;
            return true;
        }

        public bool Reassign(Person initiator, Person newAssignee)
        {
            if (Initiator != initiator || Assignee != null)
            {
                return false;
            }

            Assignee = newAssignee;
            Status = WorkTaskStatus.Назначена;
            return true;
        }

        public bool SubmitReport(Person person, string reportText)
        {
            if (Assignee != person || Status != WorkTaskStatus.ВРаботе)
            {
                return false;
            }

            Report = new Report(reportText, DateTime.Now, person);
            Status = WorkTaskStatus.НаПроверке;
            return true;
        }

        public bool ApproveReport(Person initiator)
        {
            if (Initiator != initiator || Status != WorkTaskStatus.НаПроверке)
            {
                return false;
            }

            Status = WorkTaskStatus.Выполнена;
            return true;
        }

        public bool ReturnForRevision(Person initiator)
        {
            if (Initiator != initiator || Status != WorkTaskStatus.НаПроверке)
            {
                return false;
            }

            Report = null;
            Status = WorkTaskStatus.ВРаботе;
            return true;
        }
    }

    public class Project
    {
        public string Description { get; }
        public DateTime Deadline { get; }
        public Person Customer { get; }
        public Person TeamLead { get; }
        public List<WorkTask> Tasks { get; } = new List<WorkTask>();
        public ProjectStatus Status { get; private set; }

        public Project(string description, DateTime deadline, Person customer, Person teamLead)
        {
            Description = description;
            Deadline = deadline;
            Customer = customer;
            TeamLead = teamLead;
            Status = ProjectStatus.Проект;
        }

        public WorkTask AddTask(Person initiator, string description, DateTime deadline, Person assignee)
        {
            if (Status != ProjectStatus.Проект || initiator != TeamLead)
            {
                return null;
            }

            WorkTask task = new WorkTask(description, deadline, initiator, assignee);
            Tasks.Add(task);
            return task;
        }

        public bool RemoveTask(Person initiator, WorkTask task)
        {
            if (initiator != TeamLead || task.Assignee != null)
            {
                return false;
            }

            return Tasks.Remove(task);
        }

        public bool StartExecution(Person initiator)
        {
            if (initiator != TeamLead || Status != ProjectStatus.Проект || Tasks.Count == 0)
            {
                return false;
            }

            Status = ProjectStatus.Исполнение;
            return true;
        }

        public bool CheckAndClose()
        {
            if (Status != ProjectStatus.Исполнение || Tasks.Count == 0)
            {
                return false;
            }

            if (Tasks.All(t => t.Status == WorkTaskStatus.Выполнена))
            {
                Status = ProjectStatus.Закрыт;
                return true;
            }

            return false;
        }
    }

    internal class Program
    {
        private static void Main()
        {
            Person customer = new Person("Заказчик ООО Альфа");
            Person teamLead = new Person("Тимлид Алексей");

            List<Person> team = new List<Person>
            {
                new Person("Иван"),
                new Person("Мария"),
                new Person("Дмитрий"),
                new Person("Елена"),
                new Person("Сергей"),
                new Person("Анна"),
                new Person("Павел"),
                new Person("Ольга"),
                new Person("Никита"),
                new Person("Татьяна")
            };

            Project project = new Project(
                "Разработка корпоративного портала",
                DateTime.Now.AddDays(30),
                customer,
                teamLead
            );

            Console.WriteLine($"Создан проект: {project.Description} | Статус: {project.Status}");

            for (int i = 0; i < team.Count; i++)
            {
                project.AddTask(teamLead, $"Модуль #{i + 1}", DateTime.Now.AddDays(10 + i), team[i]);
            }

            WorkTask delegatedTask = project.AddTask(teamLead, "Настройка CI/CD", DateTime.Now.AddDays(7), team[0]);
            WorkTask reassignedTask = project.AddTask(teamLead, "Интеграция почты", DateTime.Now.AddDays(8), team[1]);
            WorkTask deletedTask = project.AddTask(teamLead, "Устаревший скрипт миграции", DateTime.Now.AddDays(5), team[2]);

            project.StartExecution(teamLead);
            Console.WriteLine($"Задачи назначены ({project.Tasks.Count} шт.). Статус проекта: {project.Status}");

            delegatedTask.Delegate(team[0], team[1]);
            Console.WriteLine($"Задача \"{delegatedTask.Description}\" делегирована: {delegatedTask.Assignee.Name} | Статус: {delegatedTask.Status}");

            reassignedTask.Reject(team[1]);
            reassignedTask.Reassign(teamLead, team[3]);
            Console.WriteLine($"Задача \"{reassignedTask.Description}\" отклонена и переназначена: {reassignedTask.Assignee.Name} | Статус: {reassignedTask.Status}");

            deletedTask.Reject(team[2]);
            project.RemoveTask(teamLead, deletedTask);
            Console.WriteLine($"Задача \"{deletedTask.Description}\" отклонена и удалена тимлидом.");

            WorkTask firstTask = project.Tasks[0];
            firstTask.TakeInWork(firstTask.Assignee);
            firstTask.SubmitReport(firstTask.Assignee, "Черновая реализация модуля #1");
            firstTask.ReturnForRevision(teamLead);
            Console.WriteLine($"Отчет по задаче \"{firstTask.Description}\" возвращен на доработку | Статус: {firstTask.Status}");

            firstTask.SubmitReport(firstTask.Assignee, "Исправленная реализация модуля #1");
            firstTask.ApproveReport(teamLead);
            Console.WriteLine($"Отчет по задаче \"{firstTask.Description}\" утвержден | Статус: {firstTask.Status}");

            foreach (WorkTask task in project.Tasks)
            {
                if (task.Status == WorkTaskStatus.Выполнена)
                {
                    continue;
                }

                task.TakeInWork(task.Assignee);
                task.SubmitReport(task.Assignee, $"Работа по задаче \"{task.Description}\" выполнена");
                task.ApproveReport(teamLead);
            }

            project.CheckAndClose();
            Console.WriteLine($"Все задачи выполнены ({project.Tasks.Count} шт.). Итоговый статус проекта: {project.Status}");
        }
    }
}
