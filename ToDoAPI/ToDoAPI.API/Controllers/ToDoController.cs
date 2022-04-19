using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using ToDoAPI.API.Models;
using ToDoAPI.DATA.EF;

namespace ToDoAPI.API.Controllers
{
        //Upon creating this controller (Web API 2 controller), we need to add some functionality to the controllers.
        //1. We need to add using statements for our models
        //2. Add a using for the data layer
        //3. Install-Package Microsoft.AspNet.WebApi.Cors
        //4. Navigate to the App_Start/WebConfig.cs and add a line of code to allow Cross Origin Resource Sharing globally
        //5. Add a Cors using statement
        //6. Add the code below to limit who can request data from our API

        //In the code below we are giving permission to specific URLs(origins), specific types of data (headers), and specific methods(GET/POST/PUT/DELETE)
        //GET = READ
        //POST = CREATE
        //PUT = EDIT
        //DELETE = DELETE
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public class ToDoController : ApiController
        {
            //Create a connection to the database
            ToDoEntities db = new ToDoEntities();

            //GET - /api/resources
            public IHttpActionResult GetToDos()
            {
                //Below we create a list of Entity Framework resource objects. In an API, it is best practice to install Entity Framework to the API layer when needing to accomplish this task.
                List<ToDoViewModel> todo = db.TodoItems.Include("Category").Select(t => new ToDoViewModel()
                {
                    //Assign the columns of the Resource db table to the ResourceViewModel object, so we can use the data (send the data back to requesting app)
                    TodoId = t.TodoId,
                    Action = t.Action,
                    Done = t.Done,
                    CategoryId = t.CategoryId,
                    Category = new CategoryViewModel()
                    {
                        CategoryId = t.Category.CategoryId,
                        Name = t.Category.Name,
                        Description = t.Category.Description
                    }
                }).ToList<ToDoViewModel>();

                //Check results and handle accordingly below
                if (todo.Count == 0)
                {
                    return NotFound();
                }//Everything is good, return the data
                return Ok(todo);//resources are being passed in the responce back to the requesting app.
            }//end GetResources()

        //Get - /api/resources/id
        public IHttpActionResult GetToDo(int id)
        {
            //Create a new ResourceViewModel object and assign it the appropriate resource from the db
            ToDoViewModel ToDo = db.TodoItems.Include("Category").Where(t => t.TodoId == id).Select(t =>
                new ToDoViewModel()
                {
                    // COPY THE ASSIGNMENTS FROM THE GetResources() and paste
                    TodoId = t.TodoId,
                    Action = t.Action,
                    Done = t.Done,
                    CategoryId = t.CategoryId,
                    Category = new CategoryViewModel()
                    {
                        CategoryId = t.Category.CategoryId,
                        Name = t.Category.Name,
                        Description = t.Category.Description
                    }
                }).FirstOrDefault();

            //scopless if - once the return executes the scopes are closed.
            if (ToDo == null)
                return NotFound();

            return Ok(ToDo);
        }//end GetResource


        public IHttpActionResult PostResource(ToDoViewModel toDo)
        {
            //1. Check to validate the object - we need to know that all the data necessary to create a resource is there
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }//end if

            TodoItem newToDo = new TodoItem()
            {
                Action = toDo.Action,
                Done = toDo.Done,
                CategoryId = toDo.CategoryId,
            };

            //add the record and save changes
            db.TodoItems.Add(newToDo);
            db.SaveChanges();

            return Ok(newToDo);

        }//End Post Resource

        //PUT - api/resources (HTTPPut)
        public IHttpActionResult PutToDo(ToDoViewModel toDo)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            //Get the resource from the db so we can modify it
            TodoItem existingToDo = db.TodoItems.Where(t => t.TodoId == toDo.TodoId).FirstOrDefault();

            //modify the resource
            if (existingToDo != null)
            {
                existingToDo.Action = toDo.Action;
                existingToDo.Done = toDo.Done;
                existingToDo.CategoryId = toDo.CategoryId;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }//end PUT

        //DELETE - api/resources/id (HTTPDelete)
        public IHttpActionResult DeleteToDo(int id)
        {
            //Get the resource from the API yo make sure there's a resource with this id
            TodoItem todo = db.TodoItems.Where(t => t.TodoId == id).FirstOrDefault();

            if (todo != null)
            {
                db.TodoItems.Remove(todo);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end Delete

        //We use the Dispose() below to dispose of any connections to the database after we are done with them - best practice to handle performance - dispose of the instance of the controller and db connection when we are done with it.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }//end class
}//end namespace
