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

        public ActionResult TasksReceived(int? id=1)
        {
            var ReceivedTaskIdNotFinished = new List<int>();
            var ReceivedTaskTitlesNotFinished = new List<string>();
            var ReceivedTaskDeadlineNotFinished = new List<DateTime>();
            var ReceivedTaskPublishTimeNotFinished = new List<DateTime>();
            var ReceivedTaskExecutorsNotFinished = new List<List<String>>();
            var ReceivedTaskTagNotFinished = new List<string>();
            var ReceivedTaskStatementNotFinished = new List<int>();
            var ReceivedTaskIntroductionNotFinished = new List<string>();
            var ReceivedTaskIdFinished = new List<int>();
            var ReceivedTaskTitlesFinished = new List<string>();
            var ReceivedTaskDeadlineFinished = new List<DateTime>();
            var ReceivedTaskPublishTimeFinished = new List<DateTime>();
            var ReceivedTaskExecutorsFinished = new List<List<String>>();
            var ReceivedTaskTagFinished = new List<string>();
            var ReceivedTaskStatementFinished = new List<int>();
            var ReceivedTaskIntroductionFinished = new List<string>();

            const int pageSize = 5;
            int countNotFinished, countFinished, countTotal, beginNum;
            int pageTotal;
            countNotFinished = ReceivedTaskIdNotFinished.Count;
            countFinished = ReceivedTaskIdFinished.Count;
            countTotal = countFinished + countNotFinished;
            pageTotal = (int)Math.Ceiling((double)(countTotal / pageSize));

            ViewBag.ReceivedTaskIdPresentPage = new int[pageSize];
            ViewBag.ReceivedTaskTitlesPresentPage = new string[pageSize];
            ViewBag.ReceivedTaskDeadlinePresentPage = new DateTime[pageSize];
            ViewBag.ReceivedTaskPublishTimePresentPage = new DateTime[pageSize];
            ViewBag.ReceivedTaskExecutorsPresentPage = new List<string>[pageSize];
            ViewBag.ReceivedTaskTagPresentPage = new string[pageSize];
            ViewBag.ReceivedTaskStatementPresentPage = new int[pageSize];
            ViewBag.ReceivedTaskIntroductionPresentPage = new string[pageSize];
            ViewBag.PageNow = (int)id;
            ViewBag.PageTotal = pageTotal;

            string userId = "1552635";
            //NotFinished
            var executesNotFinished = from u in db.executeTasks
                           where u.SID == userId && u.task.executeStatement != 0
                           orderby u.task.deadlineDate
                           select u;
            foreach(var item in executesNotFinished)
            {
                ReceivedTaskIdNotFinished.Add(item.taskID);
                ReceivedTaskTitlesNotFinished.Add(item.task.taskTitle);
                ReceivedTaskDeadlineNotFinished.Add(item.task.deadlineDate);
                ReceivedTaskPublishTimeNotFinished.Add(item.task.publishDate);
                //ReceivedTaskExecutorNotFinished.Add
                var executorsNotFinished = from u in db.users
                                           from e in executesNotFinished
                                           where e.taskID == item.taskID && u.SID == e.SID
                                           select u;
                var tempList = new List<string>();
                foreach (var executors in executorsNotFinished)
                {
                    tempList.Add(executors.name);
                }
                ReceivedTaskExecutorsNotFinished.Add(tempList);
                ReceivedTaskTagNotFinished.Add(item.task.tag);
                ReceivedTaskStatementNotFinished.Add(item.task.executeStatement);
                ReceivedTaskIntroductionNotFinished.Add(item.task.introduction);
            }
            //Finished
            var executesFinished = from u in db.executeTasks
                                      where u.SID == userId && u.task.executeStatement == 0
                                      orderby u.task.deadlineDate
                                      select u;
            foreach (var item in executesFinished)
            {
                ReceivedTaskIdFinished.Add(item.taskID);
                ReceivedTaskTitlesFinished.Add(item.task.taskTitle);
                ReceivedTaskDeadlineFinished.Add(item.task.deadlineDate);
                ReceivedTaskPublishTimeFinished.Add(item.task.publishDate);
                //ReceivedTaskExecutorFinished.Add
                var executorsFinished = from u in db.users
                                           from e in executesFinished
                                           where e.taskID == item.taskID && u.SID == e.SID
                                           select u;
                var tempList = new List<string>();
                foreach (var executors in executorsFinished)
                {
                    tempList.Add(executors.name);
                }
                ReceivedTaskExecutorsFinished.Add(tempList);
                ReceivedTaskTagFinished.Add(item.task.tag);
                ReceivedTaskStatementFinished.Add(item.task.executeStatement);
                ReceivedTaskIntroductionFinished.Add(item.task.introduction);
            }

            if (id < 1)
                id =1;
            if (id > pageTotal)
                id = pageTotal;
            beginNum = pageSize * ((int)id - 1);
            if((beginNum + pageSize) <= countNotFinished)
            {
                //从notFinished中抽取 pageSize个数据出来
                for (int i = 0; i < pageSize; i++)
                {
                    ViewBag.ReceivedTaskIdPresentPage.Add(ReceivedTaskIdNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskTitlesPresentPage.Add(ReceivedTaskTitlesNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskDeadlinePresentPage.Add(ReceivedTaskDeadlineNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskPublishTimePresentPage.Add(ReceivedTaskPublishTimeNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskExecutorsPresentPage.Add(ReceivedTaskExecutorsNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskTagPresentPage.Add(ReceivedTaskTagNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskStatementPresentPage.Add(ReceivedTaskStatementNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskIntroductionPresentPage.Add(ReceivedTaskIntroductionNotFinished[beginNum + i]);
                }
            }
            else if(beginNum <= (countNotFinished - 1))
            {
                //从notFinished中抽取 (countNotFinished - 1 - beginCount - 1 + 1)个数据
                //再从Finished中抽取剩下的数量
                int notFinishedAddCount = (countNotFinished - 1) - beginNum + 1;
                int finishedAddCount = (((beginNum + pageSize) <= countTotal) ? (beginNum + pageSize) : (countTotal))
                    - countNotFinished;
                for (int i = 0; i < notFinishedAddCount; i++)
                {
                    ViewBag.ReceivedTaskIdPresentPage.Add(ReceivedTaskIdNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskTitlesPresentPage.Add(ReceivedTaskTitlesNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskDeadlinePresentPage.Add(ReceivedTaskDeadlineNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskPublishTimePresentPage.Add(ReceivedTaskPublishTimeNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskExecutorsPresentPage.Add(ReceivedTaskExecutorsNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskTagPresentPage.Add(ReceivedTaskTagNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskStatementPresentPage.Add(ReceivedTaskStatementNotFinished[beginNum + i]);
                    ViewBag.ReceivedTaskIntroductionPresentPage.Add(ReceivedTaskIntroductionNotFinished[beginNum + i]);
                }
                for (int i = 0; i < finishedAddCount; i++)
                {
                    ViewBag.ReceivedTaskIdPresentPage.Add(ReceivedTaskIdFinished[i]);
                    ViewBag.ReceivedTaskTitlesPresentPage.Add(ReceivedTaskTitlesFinished[i]);
                    ViewBag.ReceivedTaskDeadlinePresentPage.Add(ReceivedTaskDeadlineFinished[i]);
                    ViewBag.ReceivedTaskPublishTimePresentPage.Add(ReceivedTaskPublishTimeFinished[i]);
                    ViewBag.ReceivedTaskExecutorsPresentPage.Add(ReceivedTaskExecutorsFinished[i]);
                    ViewBag.ReceivedTaskTagPresentPage.Add(ReceivedTaskTagFinished[i]);
                    ViewBag.ReceivedTaskStatementPresentPage.Add(ReceivedTaskStatementFinished[i]);
                    ViewBag.ReceivedTaskIntroductionPresentPage.Add(ReceivedTaskIntroductionFinished[i]);
                }
             }
            else 
            {
                int finishedAddCount = (((beginNum + pageSize) <= countTotal) ? (beginNum + pageSize) : (countTotal))
                   - countNotFinished;
                beginNum = beginNum - countNotFinished;
                for (int i = 0; i < finishedAddCount; i++)
                {
                    ViewBag.ReceivedTaskIdPresentPage.Add(ReceivedTaskIdFinished[beginNum + 1]);
                    ViewBag.ReceivedTaskTitlesPresentPage.Add(ReceivedTaskTitlesFinished[beginNum + 1]);
                    ViewBag.ReceivedTaskDeadlinePresentPage.Add(ReceivedTaskDeadlineFinished[beginNum + 1]);
                    ViewBag.ReceivedTaskPublishTimePresentPage.Add(ReceivedTaskPublishTimeFinished[beginNum + 1]);
                    ViewBag.ReceivedTaskExecutorsPresentPage.Add(ReceivedTaskExecutorsFinished[beginNum + 1]);
                    ViewBag.ReceivedTaskTagPresentPage.Add(ReceivedTaskTagFinished[beginNum + 1]);
                    ViewBag.ReceivedTaskStatementPresentPage.Add(ReceivedTaskStatementFinished[beginNum + 1]);
                    ViewBag.ReceivedTaskIntroductionPresentPage.Add(ReceivedTaskIntroductionFinished[beginNum + 1]);
                }
            }

            return View();
        }

        public ActionResult TasksPublish(int? id=1)
        {
            var PublishTaskIdNotFinished = new List<int>();
            var PublishTaskTitlesNotFinished = new List<string>();
            var PublishTaskDeadlineNotFinished = new List<DateTime>();
            var PublishTaskPublishTimeNotFinished = new List<DateTime>();
            var PublishTaskExecutorsNotFinished = new List<List<String>>();
            var PublishTaskTagNotFinished = new List<string>();
            var PublishTaskStatementNotFinished = new List<int>();
            var PublishTaskIntroductionNotFinished = new List<string>();
            var PublishTaskIdFinished = new List<int>();
            var PublishTaskTitlesFinished = new List<string>();
            var PublishTaskDeadlineFinished = new List<DateTime>();
            var PublishTaskPublishTimeFinished = new List<DateTime>();
            var PublishTaskExecutorsFinished = new List<List<String>>();
            var PublishTaskTagFinished = new List<string>();
            var PublishTaskStatementFinished = new List<int>();
            var PublishTaskIntroductionFinished = new List<string>();

            const int pageSize = 5;
            int countNotFinished, countFinished, countTotal, beginNum;
            int pageTotal;
            countNotFinished = PublishTaskIdNotFinished.Count;
            countFinished = PublishTaskIdFinished.Count;
            countTotal = countFinished + countNotFinished;
            pageTotal = (int)Math.Ceiling((double)(countTotal / pageSize));

            ViewBag.ReceivedTaskIdPresentPage = new int[pageSize];
            ViewBag.ReceivedTaskTitlesPresentPage = new string[pageSize];
            ViewBag.ReceivedTaskDeadlinePresentPage = new DateTime[pageSize];
            ViewBag.ReceivedTaskPublishTimePresentPage = new DateTime[pageSize];
            ViewBag.ReceivedTaskExecutorsPresentPage = new List<string>[pageSize];
            ViewBag.ReceivedTaskTagPresentPage = new string[pageSize];
            ViewBag.ReceivedTaskStatementPresentPage = new int[pageSize];
            ViewBag.ReceivedTaskIntroductionPresentPage = new string[pageSize];
            ViewBag.PageNow = (int)id;
            ViewBag.PageTotal = pageTotal;

            string userId = "1552635";
            //NotFinished
            var executesNotFinished = from u in db.executeTasks
                                      from t in db.tasks
                                      where t.SID == userId && u.taskID == t.taskID && u.executeStatement != 0
                                      orderby u.task.deadlineDate
                                      select u;
            foreach (var item in executesNotFinished)
            {
                PublishTaskTitlesNotFinished.Add(item.task.taskTitle);
                PublishTaskDeadlineNotFinished.Add(item.task.deadlineDate);
                PublishTaskPublishTimeNotFinished.Add(item.task.publishDate);
                //PublishTaskExecutorNotFinished.Add
                var executorsNotFinished = from u in db.users
                                           from e in executesNotFinished
                                           where e.taskID == item.taskID && u.SID == e.SID
                                           select u;
                var tempList = new List<string>();
                foreach (var executors in executorsNotFinished)
                {
                    tempList.Add(executors.name);
                }
                PublishTaskExecutorsNotFinished.Add(tempList);
                PublishTaskTagNotFinished.Add(item.task.tag);
                PublishTaskStatementNotFinished.Add(item.task.executeStatement);
                PublishTaskIntroductionNotFinished.Add(item.task.introduction);
            }
            //Finished
            var executesFinished = from u in db.executeTasks
                                   from t in db.tasks
                                   where t.SID == userId && u.taskID == t.taskID && u.task.executeStatement == 0
                                   orderby u.task.deadlineDate
                                   select u;
            foreach (var item in executesFinished)
            {
                PublishTaskTitlesFinished.Add(item.task.taskTitle);
                PublishTaskDeadlineFinished.Add(item.task.deadlineDate);
                PublishTaskPublishTimeFinished.Add(item.task.publishDate);
                //PublishTaskExecutorFinished.Add
                var executorsFinished = from u in db.users
                                        from e in executesFinished
                                        where e.taskID == item.taskID && u.SID == e.SID
                                        select u;
                var tempList = new List<string>();
                foreach (var executors in executorsFinished)
                {
                    tempList.Add(executors.name);
                }
                PublishTaskExecutorsFinished.Add(tempList);
                PublishTaskTagFinished.Add(item.task.tag);
                PublishTaskStatementFinished.Add(item.task.executeStatement);
                PublishTaskIntroductionFinished.Add(item.task.introduction);
            }

            if (id < 1)
                id = 1;
            if (id > pageTotal)
                id = pageTotal;
            beginNum = pageSize * ((int)id - 1);
            if ((beginNum + pageSize) <= countNotFinished)
            {
                //从notFinished中抽取 pageSize个数据出来
                for (int i = 0; i < pageSize; i++)
                {
                    ViewBag.PublishTaskIdPresentPage.Add(PublishTaskIdNotFinished[beginNum + i]);
                    ViewBag.PublishTaskTitlesPresentPage.Add(PublishTaskTitlesNotFinished[beginNum + i]);
                    ViewBag.PublishTaskDeadlinePresentPage.Add(PublishTaskDeadlineNotFinished[beginNum + i]);
                    ViewBag.PublishTaskPublishTimePresentPage.Add(PublishTaskPublishTimeNotFinished[beginNum + i]);
                    ViewBag.PublishTaskExecutorsPresentPage.Add(PublishTaskExecutorsNotFinished[beginNum + i]);
                    ViewBag.PublishTaskTagPresentPage.Add(PublishTaskTagNotFinished[beginNum + i]);
                    ViewBag.PublishTaskStatementPresentPage.Add(PublishTaskStatementNotFinished[beginNum + i]);
                    ViewBag.PublishTaskIntroductionPresentPage.Add(PublishTaskIntroductionNotFinished[beginNum + i]);
                }
            }
            else if (beginNum <= (countNotFinished - 1))
            {
                //从notFinished中抽取 (countNotFinished - 1 - beginCount - 1 + 1)个数据
                //再从Finished中抽取剩下的数量
                int notFinishedAddCount = (countNotFinished - 1) - beginNum + 1;
                int finishedAddCount = (((beginNum + pageSize) <= countTotal) ? (beginNum + pageSize) : (countTotal))
                    - countNotFinished;
                for (int i = 0; i < notFinishedAddCount; i++)
                {
                    ViewBag.PublishTaskIdPresentPage.Add(PublishTaskIdNotFinished[beginNum + i]);
                    ViewBag.PublishTaskTitlesPresentPage.Add(PublishTaskTitlesNotFinished[beginNum + i]);
                    ViewBag.PublishTaskDeadlinePresentPage.Add(PublishTaskDeadlineNotFinished[beginNum + i]);
                    ViewBag.PublishTaskPublishTimePresentPage.Add(PublishTaskPublishTimeNotFinished[beginNum + i]);
                    ViewBag.PublishTaskExecutorsPresentPage.Add(PublishTaskExecutorsNotFinished[beginNum + i]);
                    ViewBag.PublishTaskTagPresentPage.Add(PublishTaskTagNotFinished[beginNum + i]);
                    ViewBag.PublishTaskStatementPresentPage.Add(PublishTaskStatementNotFinished[beginNum + i]);
                    ViewBag.PublishTaskIntroductionPresentPage.Add(PublishTaskIntroductionNotFinished[beginNum + i]);
                }
                for (int i = 0; i < finishedAddCount; i++)
                {
                    ViewBag.PublishTaskIdPresentPage.Add(PublishTaskIdFinished[i]);
                    ViewBag.PublishTaskTitlesPresentPage.Add(PublishTaskTitlesFinished[i]);
                    ViewBag.PublishTaskDeadlinePresentPage.Add(PublishTaskDeadlineFinished[i]);
                    ViewBag.PublishTaskPublishTimePresentPage.Add(PublishTaskPublishTimeFinished[i]);
                    ViewBag.PublishTaskExecutorsPresentPage.Add(PublishTaskExecutorsFinished[i]);
                    ViewBag.PublishTaskTagPresentPage.Add(PublishTaskTagFinished[i]);
                    ViewBag.PublishTaskStatementPresentPage.Add(PublishTaskStatementFinished[i]);
                    ViewBag.PublishTaskIntroductionPresentPage.Add(PublishTaskIntroductionFinished[i]);
                }
            }
            else
            {
                int finishedAddCount = (((beginNum + pageSize) <= countTotal) ? (beginNum + pageSize) : (countTotal))
                   - countNotFinished;
                beginNum = beginNum - countNotFinished;
                for (int i = 0; i < finishedAddCount; i++)
                {
                    ViewBag.PublishTaskIdPresentPage.Add(PublishTaskIdFinished[beginNum + 1]);
                    ViewBag.PublishTaskTitlesPresentPage.Add(PublishTaskTitlesFinished[beginNum + 1]);
                    ViewBag.PublishTaskDeadlinePresentPage.Add(PublishTaskDeadlineFinished[beginNum + 1]);
                    ViewBag.PublishTaskPublishTimePresentPage.Add(PublishTaskPublishTimeFinished[beginNum + 1]);
                    ViewBag.PublishTaskExecutorsPresentPage.Add(PublishTaskExecutorsFinished[beginNum + 1]);
                    ViewBag.PublishTaskTagPresentPage.Add(PublishTaskTagFinished[beginNum + 1]);
                    ViewBag.PublishTaskStatementPresentPage.Add(PublishTaskStatementFinished[beginNum + 1]);
                    ViewBag.PublishTaskIntroductionPresentPage.Add(PublishTaskIntroductionFinished[beginNum + 1]);
                }
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
