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
        private static int PatientSearch = 1;
        private static int CompareSearch = 2;
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

            ViewBag.tumLoc = new SelectList(getLocations(PatientSearch), "Value", "Text");
            ViewBag.sex = new SelectList(getSexes(PatientSearch), "Value", "Text");
            ViewBag.clss = new SelectList(getTumorTypes(PatientSearch), "Value", "Text");
            ViewBag.grade = new SelectList(getGrades(PatientSearch), "Value", "Text");

            return View(patients);
        }

        public ActionResult CompIndex(string q, string tumLoc, string clss, string grade, string sex)
        {
            var patients = from p in db.Patients select p;
            int id = Convert.ToInt32(Request["SearchType"]);
            int who = Convert.ToInt32(Request["SearchType"]);
            patients = patients.Where(p => p.isCompare);
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

            ViewBag.tumLoc = new SelectList(getLocations(CompareSearch), "Value", "Text");
            ViewBag.sex = new SelectList(getSexes(CompareSearch), "Value", "Text");
            ViewBag.clss = new SelectList(getTumorTypes(CompareSearch), "Value", "Text");
            ViewBag.grade = new SelectList(getGrades(CompareSearch), "Value", "Text");

            return View(patients);
        }

        /*public ActionResult Compare()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.displayMenu = "No";
                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
            }
            return View(db.Patients.ToList());
        }*/

        public ActionResult Results(int? id)
        {
            //string[] TargetData = new string[3];//Idea for moving data 
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

            int wSex = 100, wAge = 100, wClass = 100, wGrade = 100, wVol = 100, wLoca = 100, wConst = 100, wResp = 100, wCardio = 100, wGast = 100, wMusc = 100, wInt = 100,
                wNeuro = 100, wExer = 100, wDiet = 100, wSymp = 0, wMeds = 0, wHF = 0; //Weighted Variables
            float simMax = (wSex + wAge + wClass + wGrade + wVol + wLoca + wConst + wResp + wCardio + wGast + wMusc + wInt
                + wNeuro + wExer + wDiet + wSymp + wMeds + wHF) / 100;
            string simData = "";

            //ALGORITHM SHOULD GO HERE
            //MAKE SURE TO ONLY COMPARE AGAINST PATIENTS WHERE isCompare == false

            Patient target = new Patient();//target variable keeps most recent "similar patient" during search
            float targetSimilarity = 0;//updated variable that hold most "similar" variable
            int currEffect = 0, targetEffect = 0, count = 0; bool surgery = false;
            char[] targetRecord = { '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0' };//this is a primitive testing variable that I made to make sure its recording everything
                                                                                                                               // correctly. im going to comment these out for now
            foreach (var curr in db.Patients)
            {
                float similarity = 0;// i = 0;
                char[] record = { '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0' };


                if (patient.patientID == curr.patientID || curr.isCompare == true)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        if (patient.Sex.Equals(curr.Sex))
                        {
                            similarity += (1 * (wSex / 100));
                            //record = record.Insert(0, "1");
                            record[0] = '1';
                        }
                        if (patient.Age == curr.Age)//Range
                        {
                            similarity += (1 * (wAge / 100));
                            record[1] = '1';
                        }

                        int pVol = ((int)patient.TumorHeight) * (int)(patient.TumorLength) * (int)(patient.TumorWidth);
                        int cVol = ((int)curr.TumorHeight) * (int)(curr.TumorLength) * (int)(curr.TumorWidth);
                        if (patient.HistologicalClassification.Equals(curr.HistologicalClassification))
                        {
                            similarity += (1 * (wClass / 100));
                            record[2] = '1';
                        }
                        if (patient.TumorLength == curr.TumorLength
                          & patient.TumorWidth == curr.TumorWidth
                              & patient.TumorHeight == curr.TumorHeight
                                  & patient.TumorLocation.Equals(curr.TumorLocation))
                        {
                            foreach (TreatmentsPivot var in curr.TreatmentsPivots)
                            {
                                if (var.PossibleTreatment.Name.Equals("Surgery"))
                                {
                                    surgery = true;
                                }
                            }
                        }
                        if (pVol == cVol)
                        {
                            similarity += (1 * (wVol / 100));
                            record[4] = '1';
                        }

                        if (patient.TumorLocation.Equals(curr.TumorLocation))
                        {
                            similarity += (1 * (wLoca / 100));
                            record[5] = '1';
                        }

                        if (!patient.Constitutional.Equals(null))
                        {
                            if (patient.Constitutional.Equals(curr.Constitutional))
                            {
                                similarity += (1 * (wConst / 100));
                                record[6] = '1';
                            }
                        }
                        if (!patient.Respiratory.Equals(null))
                        {
                            if (patient.Respiratory.Equals(curr.Respiratory))
                            {
                                similarity += (1 * (wResp / 100));
                                record[7] = '1';
                            }
                        }
                        if (!patient.Cardiovascular.Equals(null))
                        {
                            if (patient.Cardiovascular.Equals(curr.Cardiovascular))
                            {
                                similarity += (1 * (wCardio / 100));
                                record[8] = '1';
                            }
                        }
                        if (!patient.Gastrointestinal.Equals(null))
                        {
                            if (patient.Gastrointestinal.Equals(curr.Gastrointestinal))
                            {
                                similarity += (1 * (wGast / 100));
                                record[9] = '1';
                            }
                        }
                        if (!patient.Musculoskeletal.Equals(null))
                        {
                            if (patient.Musculoskeletal.Equals(curr.Musculoskeletal))
                            {
                                similarity += (1 * (wMusc / 100));
                                record[10] = '1';
                            }
                        }
                        if (!patient.Integumentary.Equals(null))
                        {
                            if (patient.Integumentary.Equals(curr.Integumentary))
                            {
                                similarity += (1 * (wInt / 100));
                                record[11] = '1';
                            }
                        }
                        if (!patient.Neurologic.Equals(null))
                        {
                            if (patient.Neurologic.Equals(curr.Neurologic))
                            {
                                similarity += (1 * (wNeuro / 100));
                                record[12] = '1';
                            }
                        }
                        if (!patient.Exercize.Equals(null))
                        {
                            if (patient.Exercize.Equals(curr.Exercize))
                            {
                                similarity += (1 * (wExer / 100));
                                record[13] = '1';
                            }
                        }
                        if (!patient.Diet.Equals(null))
                        {
                            if (patient.Diet.Equals(curr.Diet))
                            {
                                similarity += (1 * (wDiet / 100));
                                record[14] = '1';
                            }
                        }
                    } catch (NullReferenceException e) { }
                }
                if (similarity > targetSimilarity)
                {
                    target = curr;
                    targetSimilarity = similarity;
                    targetRecord = record;
                }
                else if (similarity == targetSimilarity)
                {
                    currEffect = 0;
                    targetEffect = 0;
                    foreach (TreatmentsPivot sp in curr.TreatmentsPivots)
                    {
                        currEffect += sp.effectiveness;
                    }
                    foreach (TreatmentsPivot sp in target.TreatmentsPivots)
                    {
                        targetEffect += sp.effectiveness;
                    }
                    if (currEffect > targetEffect)
                    {
                        target = curr;
                        targetRecord = record;
                    }
                }
                int simResult = (int)Math.Round((similarity / simMax) * 100, 0);
                simData += (curr.patientID + "," + simResult + ",");
                count++;
            }
            string[] TargetData = simData.Split(',');
            Array.Reverse(TargetData);
            TargetData = TargetData.Skip(1).ToArray();
            Array.Reverse(TargetData);
            int[] Data = new int[count * 2];
            int c = 0;
            foreach (string x in TargetData)
            {
                int m = 69;
                int.TryParse(x, out m);
                Data[c] = m;
                c++;
            }


            string str2 = new string(targetRecord);
            patient.comparisonResults = (count + "| Patient #" + target.patientID + " was most similar. | ");// + str2 + " | ");

            if (surgery == true)
            {
                patient.comparisonResults += "Surgery, ";
            }
            foreach (TreatmentsPivot var in target.TreatmentsPivots)
            {

                string str;
                if (var.PossibleTreatment.Name.Equals("Drug"))
                {
                    try
                    {
                        str = "Drug: " + var.notes.ToString();
                        patient.comparisonResults += str + ", ";
                    }
                    catch (NullReferenceException e) { }
                }
                else if (var.PossibleTreatment.Name.Equals("Surgery"))
                {
                    continue;
                }
                else
                {
                    str = var.PossibleTreatment.Name;
                    patient.comparisonResults += str + ", ";
                }

            }
            db.SaveChanges();

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.displayMenu = "No";
                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
            }
            ViewBag.SimData = Data;
            var temp = patient.comparisonResults.Substring(patient.comparisonResults.LastIndexOf("|") + 1);
            ViewBag.CompResults = temp.Substring(0, temp.Length - 2);
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
            ViewBag.Sex = new SelectList(getSexes(null), "Value", "Text");
            ViewBag.HistologicalGrade = new SelectList(getGrades(null), "Value", "Text");
            ViewBag.HistologicalClassification = new SelectList(getTumorTypes(null), "Value", "Text");
            ViewBag.TumorLocation = new SelectList(getLocations(null), "Value", "Text");
            ViewBag.Diet = new MultiSelectList(getDietChoices(), "Value", "Text");
            ViewBag.Neurologic = new MultiSelectList(getNeurologicChoices(), "Value", "Text");
            ViewBag.Musculoskeletal = new MultiSelectList(getMusculoskeletalChoices(), "Value", "Text");
            ViewBag.Gastrointestinal = new MultiSelectList(getGastrointestinalChoices(), "Value", "Text");
            ViewBag.Cardiovascular = new MultiSelectList(getCardiovascularChoices(), "Value", "Text");
            ViewBag.Exercize = new MultiSelectList(getExercizeChoices(), "Value", "Text");
            ViewBag.Integumentary = new MultiSelectList(getIntegumentaryChoices(), "Value", "Text");
            ViewBag.Respiratory = new MultiSelectList(getRespiratoryChoices(), "Value", "Text");
            ViewBag.Constitutional = new MultiSelectList(getConstitutionalChoices(), "Value", "Text");
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

        
        public SelectListItem[] getSexes(int? searchPurpose)
        {
            SelectListItem[] sex;
            if (searchPurpose != null)
            {
                sex = new SelectListItem[4];
                SelectListItem any = new SelectListItem
                {
                    Text = "Any",
                    Value = null
                };
                sex[3] = any;
            }
            else
                sex = new SelectListItem[3];
            SelectListItem male = new SelectListItem
            {
                Text = "Male",
                Value = "M"
            };
            sex[0] = male;
            SelectListItem female = new SelectListItem
            {
                Text = "Female",
                Value = "F"
            };
            sex[1] = female;
            SelectListItem other = new SelectListItem
            {
                Text = "Other",
                Value = "O"
            };
            sex[2] = other;
            return sex;
        }

        public SelectListItem[] getGrades(int? searchPurpose)
        {
            SelectListItem[] grade;
            if (searchPurpose != null)
            {
                grade = new SelectListItem[6];
                SelectListItem any = new SelectListItem
                {
                    Text = "Any",
                    Value = null
                };
                grade[5] = any;
            }
            else
                grade = new SelectListItem[5];
            SelectListItem zero = new SelectListItem
            {
                Text = "0",
                Value = "0"
            };
            grade[0] = zero;
            SelectListItem one = new SelectListItem
            {
                Text = "1",
                Value = "1"
            };
            grade[1] = one;
            SelectListItem two = new SelectListItem
            {
                Text = "2",
                Value = "2"
            };
            grade[2] = two;
            SelectListItem three = new SelectListItem
            {
                Text = "3",
                Value = "3"
            };
            grade[3] = three;
            SelectListItem four = new SelectListItem
            {
                Text = "4",
                Value = "4"
            };
            grade[4] = four;
            return grade;
        }
        public SelectListItem[] getLocations(int? searchPurpose)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (searchPurpose == CompareSearch)
            {
                foreach (Patient p in db.Patients.Where((item) => item.isCompare && item.TumorLocation != "" && item.TumorLocation != null))
                {
                    if (list.Where((item) => item.Text == p.TumorLocation).Count() < 1)
                    {
                        SelectListItem sli = new SelectListItem
                        {
                            Text = p.TumorLocation,
                            Value = p.TumorLocation
                        };
                        list.Add(sli);
                    }
                }
            }
            else
            {
                foreach (Patient p in db.Patients.Where((item) => item.isCompare == false && item.TumorLocation != "" && item.TumorLocation != null))
                {
                    if (list.Where((item) => item.Text == p.TumorLocation).Count() < 1)
                    {
                        SelectListItem sli = new SelectListItem
                        {
                            Text = p.TumorLocation,
                            Value = p.TumorLocation
                        };
                        list.Add(sli);
                    }
                }
            }
            
            
            if (searchPurpose != null)
            {
                SelectListItem any = new SelectListItem
                {
                    Text = "Any",
                    Value = null
                };
                list.Add(any);
            }
            else
            {
                SelectListItem other = new SelectListItem
                {
                    Text = "Other",
                    Value = "Other"
                };
                list.Add(other);
            }
            return list.ToArray();
        }

        public SelectListItem[] getTumorTypes(int? searchPurpose)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (searchPurpose == CompareSearch)
            {
                foreach (Patient p in db.Patients.Where((item) => item.isCompare && item.HistologicalClassification != "" && item.HistologicalClassification != null))
                {
                    if (list.Where((item) => item.Text == p.HistologicalClassification).Count() < 1)
                    {
                        SelectListItem sli = new SelectListItem
                        {
                            Text = p.HistologicalClassification,
                            Value = p.HistologicalClassification
                        };
                        list.Add(sli);
                    }
                }
            }
            else
            {
                foreach (Patient p in db.Patients.Where((item) => item.isCompare == false && item.HistologicalClassification != "" && item.HistologicalClassification != null))
                {
                    if (list.Where((item) => item.Text == p.HistologicalClassification).Count() < 1)
                    {
                        SelectListItem sli = new SelectListItem
                        {
                            Text = p.HistologicalClassification,
                            Value = p.HistologicalClassification
                        };
                        list.Add(sli);
                    }
                }
            }
            
            
            if (searchPurpose != null)
            {
                SelectListItem any = new SelectListItem
                {
                    Text = "Any",
                    Value = null
                };
                list.Add(any);
            }
            else
            {
                SelectListItem other = new SelectListItem
                {
                    Text = "Other",
                    Value = "Other"
                };
                list.Add(other);
            }
            return list.ToArray();
        }
        public SelectListItem[] getDietChoices()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (Patient p in db.Patients.Where((item) => item.isCompare == false && item.Diet != "" && item.Diet != null))
            {
                var array = p.Diet.Split(',');
                for (int i = 0; i < array.Length; i++)
                {
                    if (list.Where((item) => item.Text == array[i]).Count() < 1)
                    {
                        SelectListItem sli = new SelectListItem
                        {
                            Text = array[i],
                            Value = array[i]
                        };
                        list.Add(sli);
                    }
                }
            }
            return list.ToArray();
        }
        public SelectListItem[] getConstitutionalChoices()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (Patient p in db.Patients.Where((item) => item.isCompare == false && item.Constitutional != "" && item.Constitutional != null))
            {
                var array = p.Constitutional.Split(',');
                for (int i = 0; i < array.Length; i++)
                {
                    if (list.Where((item) => item.Text == array[i]).Count() < 1)
                    {
                        SelectListItem sli = new SelectListItem
                        {
                            Text = array[i],
                            Value = array[i]
                        };
                        list.Add(sli);
                    }
                }
            }
            return list.ToArray();
        }
        public SelectListItem[] getRespiratoryChoices()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (Patient p in db.Patients.Where((item) => item.isCompare == false && item.Respiratory != "" && item.Respiratory != null))
            {
                var array = p.Respiratory.Split(',');
                for (int i = 0; i < array.Length; i++)
                {
                    if (list.Where((item) => item.Text == array[i]).Count() < 1)
                    {
                        SelectListItem sli = new SelectListItem
                        {
                            Text = array[i],
                            Value = array[i]
                        };
                        list.Add(sli);
                    }
                }
            }
            return list.ToArray();
        }
        public SelectListItem[] getCardiovascularChoices()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (Patient p in db.Patients.Where((item) => item.isCompare == false && item.Cardiovascular != "" && item.Cardiovascular != null))
            {
                var array = p.Cardiovascular.Split(',');
                for (int i = 0; i < array.Length; i++)
                {
                    if (list.Where((item) => item.Text == array[i]).Count() < 1)
                    {
                        SelectListItem sli = new SelectListItem
                        {
                            Text = array[i],
                            Value = array[i]
                        };
                        list.Add(sli);
                    }
                }
            }
            return list.ToArray();
        }
        public SelectListItem[] getExercizeChoices()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (Patient p in db.Patients.Where((item) => item.isCompare == false && item.Exercize != "" && item.Exercize != null))
            {
                var array = p.Exercize.Split(',');
                for (int i = 0; i < array.Length; i++)
                {
                    if (list.Where((item) => item.Text == array[i]).Count() < 1)
                    {
                        SelectListItem sli = new SelectListItem
                        {
                            Text = array[i],
                            Value = array[i]
                        };
                        list.Add(sli);
                    }
                }
            }
            return list.ToArray();
        }
        public SelectListItem[] getNeurologicChoices()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (Patient p in db.Patients.Where((item) => item.isCompare == false && item.Neurologic != "" && item.Neurologic != null))
            {
                var array = p.Neurologic.Split(',');
                for (int i = 0; i < array.Length; i++)
                {
                    if (list.Where((item) => item.Text == array[i]).Count() < 1)
                    {
                        SelectListItem sli = new SelectListItem
                        {
                            Text = array[i],
                            Value = array[i]
                        };
                        list.Add(sli);
                    }
                }
            }
            return list.ToArray();
        }
        public SelectListItem[] getIntegumentaryChoices()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (Patient p in db.Patients.Where((item) => item.isCompare == false && item.Integumentary != "" && item.Integumentary != null))
            {
                var array = p.Integumentary.Split(',');
                for (int i = 0; i < array.Length; i++)
                {
                    if (list.Where((item) => item.Text == array[i]).Count() < 1)
                    {
                        SelectListItem sli = new SelectListItem
                        {
                            Text = array[i],
                            Value = array[i]
                        };
                        list.Add(sli);
                    }
                }
            }
            return list.ToArray();
        }
        public SelectListItem[] getMusculoskeletalChoices()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (Patient p in db.Patients.Where((item) => item.isCompare == false && item.Musculoskeletal != "" && item.Musculoskeletal != null))
            {
                var array = p.Musculoskeletal.Split(',');
                for (int i = 0; i < array.Length; i++)
                {
                    if (list.Where((item) => item.Text == array[i]).Count() < 1)
                    {
                        SelectListItem sli = new SelectListItem
                        {
                            Text = array[i],
                            Value = array[i]
                        };
                        list.Add(sli);
                    }
                }
            }
            return list.ToArray();
        }
        public SelectListItem[] getGastrointestinalChoices()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (Patient p in db.Patients.Where((item) => item.isCompare == false && item.Gastrointestinal != "" && item.Gastrointestinal != null))
            {
                var array = p.Gastrointestinal.Split(',');
                for (int i = 0; i < array.Length; i++)
                {
                    if (list.Where((item) => item.Text == array[i]).Count() < 1)
                    {
                        SelectListItem sli = new SelectListItem
                        {
                            Text = array[i],
                            Value = array[i]
                        };
                        list.Add(sli);
                    }
                }
            }
            return list.ToArray();
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
                x = await db.SaveChangesAsync();
                if (patient.isCompare)
                {
                    return RedirectToAction("Details", new { id });
                }  
                return RedirectToAction("Index");
            }
            ViewBag.Sex = new SelectList(getSexes(null), "Value", "Text", patient.Sex);
            ViewBag.HistologicalGrade = new SelectList(getGrades(null), "Value", "Text", patient.HistologicalGrade);
            ViewBag.HistologicalClassification = new SelectList(getTumorTypes(null), "Value", "Text",patient.HistologicalClassification);
            ViewBag.TumorLocation = new SelectList(getLocations(null), "Value", "Text", patient.TumorLocation);
            ViewBag.Diet = new MultiSelectList(getDietChoices(), "Value", "Text");
            ViewBag.Neurologic = new MultiSelectList(getNeurologicChoices(), "Value", "Text");
            ViewBag.Musculoskeletal = new MultiSelectList(getMusculoskeletalChoices(), "Value", "Text");
            ViewBag.Gastrointestinal = new MultiSelectList(getGastrointestinalChoices(), "Value", "Text");
            ViewBag.Cardiovascular = new MultiSelectList(getCardiovascularChoices(), "Value", "Text");
            ViewBag.Exercize = new MultiSelectList(getExercizeChoices(), "Value", "Text");
            ViewBag.Integumentary = new MultiSelectList(getIntegumentaryChoices(), "Value", "Text");
            ViewBag.Respiratory = new MultiSelectList(getRespiratoryChoices(), "Value", "Text");
            ViewBag.Constitutional = new MultiSelectList(getConstitutionalChoices(), "Value", "Text");
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
            ViewBag.Sex = new SelectList(getSexes(null), "Value", "Text", patient.Sex);
            ViewBag.HistologicalGrade = new SelectList(getGrades(null), "Value", "Text", patient.HistologicalGrade);
            ViewBag.HistologicalClassification = new SelectList(getTumorTypes(null), "Value", "Text", patient.HistologicalClassification);
            ViewBag.TumorLocation = new SelectList(getLocations(null), "Value", "Text", patient.TumorLocation);
            ViewBag.Diet = new MultiSelectList(getDietChoices(), "Value", "Text");
            ViewBag.Neurologic = new MultiSelectList(getNeurologicChoices(), "Value", "Text");
            ViewBag.Musculoskeletal = new MultiSelectList(getMusculoskeletalChoices(), "Value", "Text");
            ViewBag.Gastrointestinal = new MultiSelectList(getGastrointestinalChoices(), "Value", "Text");
            ViewBag.Cardiovascular = new MultiSelectList(getCardiovascularChoices(), "Value", "Text");
            ViewBag.Exercize = new MultiSelectList(getExercizeChoices(), "Value", "Text");
            ViewBag.Integumentary = new MultiSelectList(getIntegumentaryChoices(), "Value", "Text");
            ViewBag.Respiratory = new MultiSelectList(getRespiratoryChoices(), "Value", "Text");
            ViewBag.Constitutional = new MultiSelectList(getConstitutionalChoices(), "Value", "Text");
            ViewBag.curDiet = patient.Diet;
            ViewBag.curNeurologic = patient.Neurologic;
            ViewBag.curMusculoskeletal = patient.Musculoskeletal;
            ViewBag.curGastrointestinal = patient.Gastrointestinal;
            ViewBag.curCardiovascular = patient.Cardiovascular;
            ViewBag.curExercize = patient.Exercize;
            ViewBag.curIntegumentary = patient.Integumentary;
            ViewBag.curRespiratory = patient.Respiratory;
            ViewBag.curConstitutional = patient.Constitutional;
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
            ViewBag.Sex =new SelectList(getSexes(null), "Value", "Text", patient.Sex);
            ViewBag.HistologicalGrade = new SelectList(getGrades(null), "Value", "Text", patient.HistologicalGrade);
            ViewBag.HistologicalClassification = new SelectList(getTumorTypes(null), "Value", "Text", patient.HistologicalClassification);
            ViewBag.TumorLocation = new SelectList(getLocations(null), "Value", "Text", patient.TumorLocation);
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
                return RedirectToAction("Index", "Manage");
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
