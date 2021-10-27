# SELECT
IQueryable selection automation library for c#

## LinQ query generator for c#
Applies to DB queries as well as dictionaries, lists, etc... (any class that implements IQueryable or IEnumerable)



[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://github.com/probabilty/SELECT/tree/master)


## Features

- Extremly easy to use
- Light weight
- Unify and simplfy Apis
- Multi-platform
- Works with relations, nested objectes, and Enums
- Geometry Support
- Pure c#
- Opensource
- MIT License

Markdown is an opensource free platform for automating search APIs while keeping the developer in control.
It is built using the powerful features of Expressions in c#.
Please feel free to contriute.


## Installation

Using package manager 
```sh
Install-Package SELECT -Version 1.0.0
```
Using DOTNET CLI
```sh
dotnet add package SELECT --version 1.0.0
```
Using  PackageReference
```sh
<PackageReference Include="SELECT" Version="1.0.0" />
```
## Usage
1. Create new Console Project in c# with name "test"
2. Install SELECT
    ```sh
    dotnet add package SELECT --version 1.0.0
    ```
3. Add class "Grade"
    ```c#
    public class Grade
    {
        public string subject { get; set; }
        public int grade { get; set; }
    }
    ```
4. Add class "branch"
    ```c#
    public class branch
    {
        public string name { get; set; }
        public Grade grade { get; set; }
    }
    ```
5. Add class "user"
    ```c#
    public class User
    {
        public string name { get; set; }
        public int age { get; set; }
        public branch branch { get; set; }
    }
    ```
6. In the Main function, lets add some users
    ```c#
            List<User> users = new();
            for (int i = 0; i < 10; i++)
            {
                User user = new();
                user.name = i.ToString();
                user.age = i;
                user.branch = new branch
                {
                    grade = new()
                    {
                        subject = user.name + i.ToString(),
                        grade = user.age
                    },
                    name = "test"

                };
                users.Add(user);
            }
    ```
7. Finally, lets use the package
Lets make a request that select users who are 5 years old or younger, get their age, thier grade, and thier branch name
Then order the results in a descending manner using the user's age.
    ```c#
            Request request = new()
            {
                Items = "age,branch.grade,branch.name",
                Order = new()
                {
                    IsAsc = false,
                    Name = "age"
                },
                Filters = new Filter[]
                {
                    new Filter(Operator.LtE,"age",5)
                }
            };
    ```
8. Retrive and print the results
    ```c#
          IQueryable<User> result = users.AsQueryable().Construct(request);
          Console.WriteLine(JsonConvert.SerializeObject(result));
    ```
### The complete example code
```c#
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SELECT;
using SELECT.Entities;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<User> users = new();
            for (int i = 0; i < 10; i++)
            {
                User user = new();
                user.name = i.ToString();
                user.age = i;
                user.branch = new branch
                {
                    grade = new()
                    {
                        subject = user.name + i.ToString(),
                        grade = user.age
                    },
                    name = "test"

                };
                users.Add(user);
            }
            Request request = new()
            {
                Items = "age,branch.grade,branch.name",
                order = new()
                {
                    IsAsc = false,
                    name = "age"
                },
                filters = new Filter[]
                {
                    new Filter(Operator.LtE,"age",5)
                }
            };
            IQueryable<User> result = users.AsQueryable().Construct(request);
            Console.WriteLine(JsonConvert.SerializeObject(result));
        }
    }

    public class User
    {
        public string name { get; set; }
        public int age { get; set; }
        public branch branch { get; set; }
    }
    public class branch
    {
        public string name { get; set; }
        public Grade grade { get; set; }
    }
    public class Grade
    {
        public string subject { get; set; }
        public int grade { get; set; }
    }
}
```
## The Construct Method
This method acts on IQueryable objects, and recives a Request Object
the Reuest Object consists of 3 Properties:
1. Items is a comma separated string, where each substing contains a field name
   Nested Objectes can be accessed using the ".". For example; if you need to select ID, Name and Car model, the items string should be 
   ``` c#
    Items = "Id,Name,Car.Model"
   ```
2. The order property is an object of type Order. This Object is resposable for ordering the result. This Object Contains 2 Properties

    | Property | Type |Description |
    | ------ | ------ |------ |
    | name | string | The name of the property that is selected to order the reslts by|
    | IsAsc | bool |If true The order is ascending, else the order is descending |

3. The Filters Property is an array of the filter object. This specifies what result to reurn. Ie Applies its filters to the IQueryable object that acts on it. The filter Object is composed of 3 Components:

    | Property | Type |Description |
    | ------ | ------ |------ |
    | fieldName | string | The field name we are setting a filter condition to |
    | value | object | The value we filtering againest |
    | op | Operator | The Operator used for filtring|
    
    The operator object is an Enum Contains these operators
    
    | Operator |Description |
    | ------ | ------ |
    | Eq | Equal |
    | Lt |  Less Than |
    | Gt |  Greater Than|
    | In | In |
    | Contains |  Contains |
    | GtE |  Greater Than or Equal|
    | LtE |   Less Than  or Equal|
    
    For Example If we need users Whos Id's is less than 5, The Filter object will be:
    ```c#
    filters = new Filter[]
    {
        new Filter(Operator.Lt,"Id",5)
    }
    ```
    Or:
     ```c#
    filters = new Filter[]
    {
        new Filter(Operator.In,"Id",new long[]{1,2,3,4})
    }
    ```
    IF we need to get the Users Whos were last seen in a Egypt
    ```c#
            filters = new Filter[]
            {
                new Filter(Operator.In,
                "Location",
                "POLYGON((-335.1708984375 29.382175075145298,-334.9951171875 31.54108987958584,-325.7666015625 31.35363694150098,-325.01953125 29.535229562948473,-326.03027343749994 26.980828590472115,-323.0419921875 21.902277966668635,-334.9951171875 21.902277966668635,-335.1708984375 29.382175075145298))")
            }
    ```
