# BetHelper-ApplicationServer
Web API 2.0 web services for back-end of SPA application<br />
Football SPA back-end server<br />
-BetHelper.Web-web services<br /> 
-DemoRight project- console application deployed as Azure continuous job for processing data polling and SignalR pushing. It is demo variant of the worker whole application.<br />
-BetHelper.Worker- console application contains whole logic for background processing- in addition to DemoRight features it uses scheduler for executing polling,SignalR pushing,<br />
Server cache pushing, and DB writing. Future(new season) implementation in Azure Web worker role.<br />
-BetHelper.TaskScheduler-scheduler for tasks used in BetHelper.Worker, perfect for Azure Web Worker role<br />