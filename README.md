# UWPandAccuWeather
This is for using AccuWeather API on UWP, which is build by Unity.
It includes two main parts, httpclient connecting to AccuWeather and NewtoneJson ecoding JSON string.
Because Windows.Web namespace isn't supported in Unity Editor, we need to use command: "if !Unity.Editor".


 I use two trick to build into HoloLens(UWP) successfuly. 

 1: One is HttpClient which is under namespace Windows.Web.Http, and it get the json string from the uri with "async" and "await". 
    It will return Task<string> type, so it needs to use ".Result".
 2: Another one is about Json. I used litJson at first, but it returned some errors because UWP doesn't support it (namespace: System.Thread).
    So I found another json.dll : JSON.NET. It is worth noting that the version of JSON.NET should be Portable40, which can support UWP8 (possibly UWP10).
    But it can not process the string forms [......], so we need to truncate them.
