using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TJSSESU_Website.DAL;
using TJSSESU_Website.Models;

namespace TJSSESU_Website.Controllers
{
    public class UsersController : Controller
    {
        private WebsiteContext db = new WebsiteContext();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.users.Include(u => u.department).Include(u => u.position);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.deptName = new SelectList(db.departments, "deptName", "introduction");
            ViewBag.posName = new SelectList(db.positions, "posName", "posName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SID,password,deptName,posName")] User user)
        {
            if (ModelState.IsValid)
            {
                db.users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.deptName = new SelectList(db.departments, "deptName", "introduction", user.deptName);
            ViewBag.posName = new SelectList(db.positions, "posName", "posName", user.posName);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.deptName = new SelectList(db.departments, "deptName", "introduction", user.deptName);
            ViewBag.posName = new SelectList(db.positions, "posName", "posName", user.posName);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SID,password,deptName,posName")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.deptName = new SelectList(db.departments, "deptName", "introduction", user.deptName);
            ViewBag.posName = new SelectList(db.positions, "posName", "posName", user.posName);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.users.Find(id);
            db.users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult TasksReceived()
        {
            ViewBag.TaskTitlesNotFinished = new List<string>();
            ViewBag.TaskDeadlineNotFinished = new List<DateTime>();
            ViewBag.TaskPublishTimeNotFinished = new List<DateTime>();
            ViewBag.TaskExecutorsNotFinished = new List<List<String>>();
            ViewBag.TaskTagNotFinished = new List<string>();
            ViewBag.TaskStatementNotFinished = new List<int>();
            ViewBag.TaskIntroductionNotFinished = new List<string>();
            ViewBag.TaskTitlesFinished = new List<string>();
            ViewBag.TaskDeadlineFinished = new List<DateTime>();
            ViewBag.TaskPublishTimeFinished = new List<DateTime>();
            ViewBag.TaskExecutorsFinished = new List<List<String>>();
            ViewBag.TaskTagFinished = new List<string>();
            ViewBag.TaskStatementFinished = new List<int>();
            ViewBag.TaskIntroductionFinished = new List<string>();
            string userId = "1552635";
            //NotFinished
            var executesNotFinished = from u in db.executeTasks
                           where u.SID == userId && u.task.executeStatement != 0
                           orderby u.task.deadlineDate
                           select u;
            foreach(var item in executesNotFinished)
            {
                ViewBag.TaskTitlesNotFinished.Add(item.task.taskTitle);
                ViewBag.TaskDeadlineNotFinished.Add(item.task.deadlineDate);
                ViewBag.TaskPublishTimeNotFinished.Add(item.task.publishDate);
                //ViewBag.TaskExecutorNotFinished.Add
                var executorsNotFinished = from u in db.users
                                           from e in executesNotFinished
                                           where e.taskID == item.taskID && u.SID == e.SID
                                           select u;
                var tempList = new List<string>();
                foreach (var executors in executorsNotFinished)
                {
                    tempList.Add(executors.name);
                }
                ViewBag.TaskExecutorsNotFinished.Add(tempList);
                ViewBag.TaskTagNotFinished.Add(item.task.tag);
                ViewBag.TaskStatementNotFinished.Add(item.task.executeStatement);
                ViewBag.TaskIntroductionNotFinished.Add(item.task.introduction);
            }
            //Finished
            var executesFinished = from u in db.executeTasks
                                      where u.SID == userId && u.task.executeStatement == 0
                                      orderby u.task.deadlineDate
                                      select u;
            foreach (var item in executesFinished)
            {
                ViewBag.TaskTitlesFinished.Add(item.task.taskTitle);
                ViewBag.TaskDeadlineFinished.Add(item.task.deadlineDate);
                ViewBag.TaskPublishTimeFinished.Add(item.task.publishDate);
                //ViewBag.TaskExecutorFinished.Add
                var executorsFinished = from u in db.users
                                           from e in executesFinished
                                           where e.taskID == item.taskID && u.SID == e.SID
                                           select u;
                var tempList = new List<string>();
                foreach (var executors in executorsFinished)
                {
                    tempList.Add(executors.name);
                }
                ViewBag.TaskExecutorsFinished.Add(tempList);
                ViewBag.TaskTagFinished.Add(item.task.tag);
                ViewBag.TaskStatementFinished.Add(item.task.executeStatement);
                ViewBag.TaskIntroductionFinished.Add(item.task.introduction);
            }
            return View();
        }

        public ActionResult TasksPublish()
        {
            ViewBag.PublishTaskTitlesNotFinished = new List<string>();
            ViewBag.PublishTaskDeadlineNotFinished = new List<DateTime>();
            ViewBag.PublishTaskPublishTimeNotFinished = new List<DateTime>();
            ViewBag.PublishTaskExecutorsNotFinished = new List<List<String>>();
            ViewBag.PublishTaskTagNotFinished = new List<string>();
            ViewBag.PublishTaskStatementNotFinished = new List<int>();
            ViewBag.PublishTaskIntroductionNotFinished = new List<string>();
            ViewBag.PublishTaskTitlesFinished = new List<string>();
            ViewBag.PublishTaskDeadlineFinished = new List<DateTime>();
            ViewBag.PublishTaskPublishTimeFinished = new List<DateTime>();
            ViewBag.PublishTaskExecutorsFinished = new List<List<String>>();
            ViewBag.PublishTaskTagFinished = new List<string>();
            ViewBag.PublishTaskStatementFinished = new List<int>();
            ViewBag.PublishTaskIntroductionFinished = new List<string>();
            string userId = "1552635";
            //NotFinished
            var executesNotFinished = from u in db.executeTasks
                                      from t in db.tasks
                                      where t.SID == userId && u.taskID == t.taskID && u.executeStatement != 0
                                      orderby u.task.deadlineDate
                                      select u;
            foreach (var item in executesNotFinished)
            {
                ViewBag.PublishTaskTitlesNotFinished.Add(item.task.taskTitle);
                ViewBag.PublishTaskDeadlineNotFinished.Add(item.task.deadlineDate);
                ViewBag.PublishTaskPublishTimeNotFinished.Add(item.task.publishDate);
                //ViewBag.PublishTaskExecutorNotFinished.Add
                var executorsNotFinished = from u in db.users
                                           from e in executesNotFinished
                                           where e.taskID == item.taskID && u.SID == e.SID
                                           select u;
                var tempList = new List<string>();
                foreach (var executors in executorsNotFinished)
                {
                    tempList.Add(executors.name);
                }
                ViewBag.PublishTaskExecutorNotFinished.Add(tempList);
                ViewBag.PublishTaskTagNotFinished.Add(item.task.tag);
                ViewBag.PublishTaskStatementNotFinished.Add(item.task.executeStatement);
                ViewBag.PublishTaskIntroductionNotFinished.Add(item.task.introduction);
            }
            //Finished
            var executesFinished = from u in db.executeTasks
                                   from t in db.tasks
                                   where t.SID == userId && u.taskID == t.taskID && u.task.executeStatement == 0
                                   orderby u.task.deadlineDate
                                   select u;
            foreach (var item in executesFinished)
            {
                ViewBag.PublishTaskTitlesFinished.Add(item.task.taskTitle);
                ViewBag.PublishTaskDeadlineFinished.Add(item.task.deadlineDate);
                ViewBag.PublishTaskPublishTimeFinished.Add(item.task.publishDate);
                //ViewBag.PublishTaskExecutorFinished.Add
                var executorsFinished = from u in db.users
                                        from e in executesFinished
                                        where e.taskID == item.taskID && u.SID == e.SID
                                        select u;
                var tempList = new List<string>();
                foreach (var executors in executorsFinished)
                {
                    tempList.Add(executors.name);
                }
                ViewBag.PublishTaskExecutorFinished.Add(tempList);
                ViewBag.PublishTaskTagFinished.Add(item.task.tag);
                ViewBag.PublishTaskStatementFinished.Add(item.task.executeStatement);
                ViewBag.PublishTaskIntroductionFinished.Add(item.task.introduction);
            }
            return View();
        }

        public ActionResult TaskResult()
        {
            
            return View();
        }

        public ActionResult TaskCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateTask()
        {
            bool resultAll = true;
            string publisherId = "1552607";
            string taskTitle = Request.Form["title"];
            string executorsId = Request.Form["IDs"];
            string tag = Request.Form["belongTo"];
            DateTime publishedDate = DateTime.Now;
            var stringDate = Request.Form["data"];
            string[] splitStringDate = stringDate.Split('/');
            if (splitStringDate[0].Length == 1)
            {
                splitStringDate[0] = "0" + splitStringDate[0];
            }
           if(splitStringDate[1].Length == 1)
           {
               splitStringDate[1] = "0" + splitStringDate[1];
           }
           stringDate = string.Join("/", splitStringDate);
            DateTime date = DateTime.ParseExact(stringDate, "MM/dd/yyyy", System.Globalization.CultureInfo.CurrentCulture);
            DateTime deadlineDate = date;
            string description = Request.Form["description"];
            bool idJudge = JudgeExecutors(executorsId, publisherId);
            if (idJudge)
            {
                var newTask = new Task
                {
                    taskTitle = taskTitle,
                    introduction = description,
                    publishDate = publishedDate,
                    deadlineDate = deadlineDate,
                    executeStatement = 1,
                    SID = publisherId,
                    tag = tag
                };
                if (ModelState.IsValid)
                {
                    db.tasks.Add(newTask);
                    db.SaveChanges();
                }
                string[] splitExecutorsId = executorsId.Split(' ');
                for (int i = 0; i < splitExecutorsId.Length; i++)
                {
                    var newExecuteTask = new ExecuteTask
                    {
                        SID = splitExecutorsId[i],
                        executeStatement = 1,
                        taskID = newTask.taskID
                    };
                    if (ModelState.IsValid)
                    {
                        db.tasks.Add(newTask);
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                resultAll = false;
            }
            if (resultAll)
            {
                return Redirect("/Users/TaskResult");
            }
            else
            {
                return null;
            }
        }

        public ActionResult CreateTaskSuccessful()
        {
            return View();
        }
        public ActionResult CreateTaskFailed()
        {
            return View();
        }

        private bool JudgeExecutors(string executorsId, string publisherId)
        {
            string[] splitExecutorsId = executorsId.Split(' ');
            string tempId;
            bool tempJudge;
            for (int i = 0; i < splitExecutorsId.Length; i++)
            {
                tempId = splitExecutorsId[i];
                tempJudge = JudgeAnExecutor(tempId, publisherId);
                if (!tempJudge)
                {
                    return false;
                }
            }
            return true;
        }

        private bool JudgeAnExecutor(string executorId, string publisherId)
        {
            bool deptJudge = false;
            bool posJudge = false;
            bool lawJudge = false;
            var publisher = from u in db.users
                            from p in db.positions
                            where u.SID == publisherId && u.posName == p.posName
                            select new
                            {
                                u.SID,
                                u.name,
                                u.posName,
                                u.deptName,
                                p.pClass
                            };
            var executor = from u in db.users
                           from p in db.positions
                           where u.SID == executorId && u.posName == p.posName
                           select new
                           {
                               u.SID,
                               u.name,
                               u.posName,
                               u.deptName,
                               p.pClass
                           };
            var publisherList = publisher.ToList();
            var executorList = executor.ToList();
            if(executorList.Count() == 0)
            {
                return false;
            }
            else
            {
                lawJudge = true;
            }
            if(publisherList[0].deptName == "主席团")
            {
                deptJudge = true;
            }
            else
            {
                if(publisherList[0].deptName != executorList[0].deptName)
                {
                    return false;
                }
                else
                {
                    deptJudge = true;
                }
            }
            if (publisherList[0].pClass <= executorList[0].pClass)
            {
                return false;
            }
            else
            {
                posJudge = true;
            }

            return (lawJudge && deptJudge && posJudge);
        }
    }
}
