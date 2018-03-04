﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TumorTaskforce_Webapp_1;
using TumorTaskforce_Webapp_1.Models;

namespace TumorTaskforce_Webapp_1.Controllers
{
    public class PatientsController : Controller
    {
        private tumorDBEntities db = new tumorDBEntities();

        // GET: Patients
        public ActionResult Index(string q, string tumLoc, string clss, string grade, string sex)
        {
            var patients = from p in db.Patients select p;
            int id = Convert.ToInt32(Request["SearchType"]);
            int who = Convert.ToInt32(Request["SearchType"]);
            patients = patients.Where(p => p.isCompare == false);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.displayMenu = "No";
                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                int pID = int.Parse(q);
                patients = patients.Where(p => p.patientID.Equals(pID));
            }

            if (!string.IsNullOrWhiteSpace(tumLoc))
            {
                patients = patients.Where(r => r.TumorLocation.Contains(tumLoc));
            }

            if (!string.IsNullOrWhiteSpace(clss))
            {
                patients = patients.Where(s => s.HistologicalClassification.Contains(clss));
            }

            if (!string.IsNullOrWhiteSpace(grade))
            {
                int hisGrade = int.Parse(grade);
                patients = patients.Where(t => t.HistologicalGrade == hisGrade);
            }

            if (!string.IsNullOrWhiteSpace(sex))
            {
                patients = patients.Where(u => u.Sex.Contains(sex));
            }

            return View(patients);          
        }

        public ActionResult CompIndex(/*string sortingMethod*/string q)
        {
            var patients = from p in db.Patients select p;
            int id = Convert.ToInt32(Request["SearchType"]);
            var searchParam = "Searching";
            patients = patients.Where(p => p.isCompare == true);

            if (!string.IsNullOrWhiteSpace(q))
            {
                switch (id)

                {
                    case 0:
                        int pID = int.Parse(q);
                        patients = patients.Where(p => p.patientID.Equals(pID));
                        searchParam += " ID for ' " + q + " ' ";
                        break;
                    /* case 1:
                         int A = Int32.Parse(q);
                         patients = patients.Where(p => p.Age.Equals(A));
                         searchParam += " Age for ' " + q + " ' ";
                         break; */
                    /*case 2:
                        patients = patients.Where(p => p.Sex.Contains(q));
                        searchParam += " Sex for ' " + q + " ' ";
                        break;*/
                    case 3:
                        patients = patients.Where(p => p.HistologicalClassification.Contains(q));
                        searchParam += " HistologicalClassification for ' " + q + " ' ";
                        break;
                }
            }
            else
            {
                searchParam += "ALL";
            }
            ViewBag.SearchParameter = searchParam;
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.displayMenu = "No";
                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
            }
            return View(patients);

        }

        //public ActionResult Compare()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        ViewBag.displayMenu = "No";
        //        if (isAdminUser())
        //        {
        //            ViewBag.displayMenu = "Yes";
        //        }
        //    }
        //    return View(db.Patients.ToList());
        //}

        public ActionResult Results(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            if (!patient.isCompare)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }


            //ALGORITHM SHOULD GO HERE
            //MAKE SURE TO ONLY COMPARE AGAINST PATIENTS WHERE isCompare == false

            //var tuple = new Tuple<TumorTaskforce_Webapp_1.Patient, IEnumerable<TumorTaskforce_Webapp_1.Patient>>(patient, db.Patients.ToList());

            //PUT SUGGESTED TREATMENTS AS STRING INTO patient.comparisonResults


            patient.comparisonResults = "Our Comparison Algorithm is Under Contruction! Check back soon. Sorry for any inconvenience.";
            db.SaveChanges();


            if (User.Identity.IsAuthenticated)
            {
                ViewBag.displayMenu = "No";
                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
            }
            return View(patient);
        }
        
        // GET: Patients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            ViewBag.isCompare = patient.isCompare;
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.displayMenu = "No";
                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
            }
            return View(patient);
        }
       

        // GET: Patients/Create
        public ActionResult Create(bool? isCompare)
        {
            if (isCompare == null) { isCompare = false; }
            ViewBag.isCompare = isCompare;
            ViewBag.Sex = new SelectList(getSexes(), "Value", "Text");
            ViewBag.HistologicalGrade = new SelectList(getGrades(), "Value", "Text");
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.displayMenu = "No";
                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
            }
            return View();
        }

        
        public SelectListItem[] getSexes()
        {
            SelectListItem[] sex = new SelectListItem[3];
            SelectListItem male = new SelectListItem();
            male.Text = "Male";
            male.Value = "M";
            sex[0] = male;
            SelectListItem female = new SelectListItem();
            female.Text = "Female";
            female.Value = "F";
            sex[1] = female;
            SelectListItem other = new SelectListItem();
            other.Text = "Other";
            other.Value = "O";
            sex[2] = other;
            return sex;
        }

        public SelectListItem[] getGrades()
        {
            SelectListItem[] grade = new SelectListItem[5];
            SelectListItem zero = new SelectListItem();
            zero.Text = "0";
            zero.Value = "0";
            grade[0] = zero;
            SelectListItem one = new SelectListItem();
            one.Text = "1";
            one.Value = "1";
            grade[1] = one;
            SelectListItem two = new SelectListItem();
            two.Text = "2";
            two.Value = "2";
            grade[2] = two;
            SelectListItem three = new SelectListItem();
            three.Text = "3";
            three.Value = "3";
            grade[3] = three;
            SelectListItem four = new SelectListItem();
            four.Text = "4";
            four.Value = "4";
            grade[4] = four;
            return grade;
        }

        // POST: Patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Create([Bind(Include = "patientID,Sex,Married,Age,HistologicalClassification,HistologicalGrade,TumorWidth,TumorHeight,TumorLength,TumorLocation,Constitutional,Respiratory,Cardiovascular,Gastrointestinal,Musculoskeletal,Integumentary,Neurologic,Exercize,Diet,isCompare,comparisonResults,userName")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Patients.Add(patient);
                //db.SaveChanges();
                int x = await db.SaveChangesAsync();
                int id = patient.patientID;
                if (patient.isCompare)
                {
                    return RedirectToAction("Details", new { id });
                }  
                return RedirectToAction("Index");
            }
            ViewBag.Sex = new SelectList(getSexes(), "Value", "Text", patient.Sex);
            ViewBag.HistologicalGrade = new SelectList(getGrades(), "Value", "Text", patient.HistologicalGrade);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.displayMenu = "No";
                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
            }
            return View(patient);
        }

        // GET: Patients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            ViewBag.isCompare = patient.isCompare;
            ViewBag.Sex = new SelectList(getSexes(), "Value", "Text", patient.Sex);
            ViewBag.HistologicalGrade = new SelectList(getGrades(), "Value", "Text", patient.HistologicalGrade);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.displayMenu = "No";
                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "patientID,Sex,Married,Age,HistologicalClassification,HistologicalGrade,TumorWidth,TumorHeight,TumorLength,TumorLocation,Constitutional,Respiratory,Cardiovascular,Gastrointestinal,Musculoskeletal,Integumentary,Neurologic,Exercize,Diet,isCompare,userName,comparisonResults")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = patient.patientID });
            }
            ViewBag.Sex =new SelectList(getSexes(), "Value", "Text", patient.Sex);
            ViewBag.HistologicalGrade = new SelectList(getGrades(), "Value", "Text", patient.HistologicalGrade);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.displayMenu = "No";
                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.displayMenu = "No";
                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
            }
            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patient patient = db.Patients.Find(id);
            bool isCompare = patient.isCompare;
            foreach( SymptomsPivot sp in db.SymptomsPivots){
                if (sp.patientID == id)
                {
                    db.SymptomsPivots.Remove(sp);
                }
            }
            foreach (TreatmentsPivot tp in db.TreatmentsPivots)
            {
                if (tp.patientID == id)
                {
                    db.TreatmentsPivots.Remove(tp);
                }
            }
            foreach (HealthFactorsPivot hp in db.HealthFactorsPivots)
            {
                if (hp.patientID == id)
                {
                    db.HealthFactorsPivots.Remove(hp);
                }
            }
            foreach (OtherMedsPivot op in db.OtherMedsPivots)
            {
                if (op.patientID == id)
                {
                    db.OtherMedsPivots.Remove(op);
                }
            }
            foreach (FamilyHistoryPivot fp in db.FamilyHistoryPivots)
            {
                if (fp.patientID == id)
                {
                    db.FamilyHistoryPivots.Remove(fp);
                }
            }
            db.SaveChanges();
            db.Patients.Remove(patient);
            db.SaveChanges();
            if (isCompare)
            {
                return RedirectToAction("Index", "Home");
            }
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

        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}
