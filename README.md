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
Install-Package SELECT -Version 0.1.0
```
Using DOTNET CLI
```sh
dotnet add package SELECT --version 0.1.0
```
Using  PackageReference
```sh
<PackageReference Include="SELECT" Version="0.1.0" />
```
## Usage
1. Create new Console Project in c# with name "test"
2. Install SELECT
    ```sh
    dotnet add package SELECT --version 0.1.0
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
