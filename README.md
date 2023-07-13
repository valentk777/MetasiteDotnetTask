# MetasiteDotnetTask
Metasite-dotnet-task

# Task description at a time this task implemented.

### Task Description:

Build console app metaapp.exe that will show and save current weather data received from API

* This command should get & save cities weather data periodically in 30 sec intervals: 

```metaapp.exe weather --city city1,city2,...,cityn.```

* Example: 

```metaapp.exe weather --city Vilnius,Brussels```

* App should display city weather data
* Weather data should be saved in a persistent way
* Multi-threaded solution should be used

For now app is small & simple, but it should be architected to be able to adapt the current app code very easily to the upcoming requirements:

* Persistent data storage type/libraries might change
* API might change
* City weather data might be displayed differently
* More console arguments might be introduced
* App might become a Web app

This does not mean that the solution should be overengineered, don’t try to impress us with your code, just create a simple architecture that can easily scale with the upcoming requirements.

If you know a nice library/framework that can make your life easier use it!

### API:

How to get weather data: https://weather-api.m3tasite.net/index.html  
Use username “meta” and password “site” for authorization.

### Requirements:

* Use .NET Core/.NET 5+
* Write high quality code. Code quality is very important to us.
* Implement logging in your app

### Bonus points:

* Write automatic tests
* Make use of AOP

### Task submission:

The results must be submitted to jobs@metasite.net.  
Source code must be placed in public version control platform (such as github, bitbucket and etc.).

# Original task source and maybe updated description:
https://github.com/Metasiteorg/net-task