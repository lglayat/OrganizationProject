# Organization Project

The project is a ASP.NET Core Web Application written in C#.

The purpose of this project is to host a single endpoint that returns a summary of a list of Organizations.

This data will be returned in the form of a JSON object with other objects nested inside of it. 

A sample of the data structure detailed below:

```
[{
	"id": "1",
	"name": "Luettgen LLC",
	"blacklistTotal": "2",
	"totalCount": "5",
	"users": [{
			"id": "1",
			"email": "Reanna78@hotmail.com",
			"phoneCount": 3
		},{
			"id": "3",
			"name": "Benton23@gmail.com",
			"phoneCount": 2
		}]
	},{
	"id": "2",
	…
}]  
```

## Approach

This application gets its data from another RESTFUL resource which throttles its API limit to 15 requests in 20 second intervals.

The exponential backup approach with Jitter was utilized to counteract any issues with throttling.

The formula to calculate the amount of time to pause before sending another request is... 
```
sleep = random_between(0, min(cap, base * 2 ** attempt))
```
I decided that the hard cap in this sample would be 32 seconds.

