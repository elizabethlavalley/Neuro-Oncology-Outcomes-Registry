
## Objective
This web app is built with the intention that medical personnel could use this as a tool to determine a patient's overall health outcomes post surgery. More specifically, diagnosed patients with brain tumors. 

Our way of helping their cause is by creating a web app that will allow medical personnel to input key information from pre and post operative visits into a database. Then, with the algorithm we developed to evaluate the tumors location, and type among other common variables; which will be used to compare against the patients in the database. 

Finally, the web app will provide medical personnel a visual demonstration of the results gathered from the comparison analysis.


## General Info.

This solution houses all the code for Tumor Taskforce's "Tumor App."

The code for the Web App is .NET MVC based and connects to a SQL database hosted on Microsoft Azure. 
The Web App is also deployed via Microsoft Azure.
Simply go to tumor1.azurewebsites.net to access the latest version of the web app.

Agorithm code is under the class named Results within the PatientsController.cs file found in the Controllers file part of the solution.

"TUMOR APP"
V 1.0

Features:
Create, view, and edit patient information, including:
	General Info
	Tumor Info
	Review of health systems
	Symptoms
	Health Factors
	Other Medications
	Treatments

Search for a patient
	Via ID
	Via Tumor Classification

Get a suggested treatment option
	Compare a selected patient with other patients
	Receive a suggested treatment option based on comparison with similar patients.

 

## Testing:

To run behavioral and unit tests:

	1) Download Visual Studios Community 2017 from the Visual Studios website 
	
	2) Download the project from Github and open it in Vs
	
	3) Install the following nuget packages in the "UnitTestProjct1" project by right clicking on the UnitTestProject1 to select "Manage Nuget Packages" from thr dropdown menu:
	**This is the most friendly method**
		
		-NUnit
		
		-NUnit3TestAdapter
		
		-NUnitTestAdapter.WithFramework
		
		-Github extension for VS **makes source control easier to use**
		
		-Selenium.WebDriver
		
		-Selenium.Firfox.WebDriver
		
		-You will need the following headers in most test files:
			
			using System;
			
			using System.Text;
			
			using System.Text.RegularExpressions;
			
			using System.Threading;
			
			using System.Web.SessionState;

			using System.IO;

			using System.Reflection;

			using System.Web;
			
			using NUnit.Framework;
			
			using OpenQA.Selenium;
			
			using OpenQA.Selenium.Firefox;
			
			using OpenQA.Selenium.Support.UI;
		
	4) Build the solution inside Visual Studios and this will activate the built in Test Explorer. If Test Explorer does not appear, simply use the shortcut key: Ctrl + E followed by T to open the Test Explorer.
	
	5) The available tests in the project will appear in a separate window that will pop-up on the side of the screen
	
	6) You can choose to run them individually or click the "Run All" button to automatically see whether the tests pass
	**In the future we plan on having the tests run automatically when the project is built and show errors if any portion of the build is unsuccessful**
	
	**You don't need any access to the database we created usign MySql because it's not necessary to run any of the tests created**
