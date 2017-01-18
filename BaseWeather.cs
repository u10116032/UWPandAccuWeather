using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

#if !UNITY_EDITOR
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using Windows.Web.Http;
using System.Threading.Tasks;
#endif

/*
 I use two trick to build into HoloLens successfuly. 

 1: One is HttpClient which is under namespace Windows.Web.Http, and it get the json string from the uri with "async" and "await". 
    It will return Task<string> type, so it needs to use ".Result".
 2: Another one is about Json. I used litJson at first, but it returned some errors because UWP doesn't support it (namespace: System.Thread).
    So I found another json.dll : JSON.NET. It is worth noting that the version of JSON.NET should be Portable40, which can support UWP8 (possibly UWP10).
    But it can not process the string forms [......], so we need to truncate them.

 */

public class BaseWeather {

	public const string API_KEY = "YOUR AccuWeahter API KEY";
	public const int TEXT_LOCATION = 0;
	public const int IP_LOCATION = 1;

}


#if !UNITY_EDITOR
public class CurrentData{
    


	HttpClient httpClient = new HttpClient();

    private JObject currentData = null;
    public  CurrentData(int type_location, string TextorIp)
    {

        string locationKey = null;

	    HttpClient httpClient = new HttpClient();
    

        try
        {
		    
			switch (type_location) {
			case BaseWeather.TEXT_LOCATION:
				{
					Uri requestUri = new Uri("http://dataservice.accuweather.com/locations/v1/cities/autocomplete?apikey="+BaseWeather.API_KEY+"&q="+TextorIp);				
					string s = getJsonString(requestUri).Result;
                    s = s.Substring(1, s.Length-2 );

                    Debug.Log("string from httpclient: " + s);
					JObject locationData = JObject.Parse (s);
                    Debug.Log("finish parsing: ");

					if (s == "[]") 
					{
						Debug.Log("getLocationKey: no Key string from uri.");
						return ;
					}

					locationKey = (string)locationData["Key"];
					break;
				}
			case BaseWeather.IP_LOCATION:
				{
					Uri requestUri = new Uri("http://dataservice.accuweather.com/locations/v1/cities/ipaddress?apikey="+BaseWeather.API_KEY+"&q="+TextorIp);
					string s =  getJsonString(requestUri).Result;

                    JObject locationData = JObject.Parse (s);

					if(locationData["Key"] == null)
						Debug.Log("getLocationKey: no Key string from uri.");

					locationKey = (string)locationData["Key"];
					break;
				}
			default:
				Debug.Log ("getLocationKey: TYPE ERROR!");
				break;
			}
			}
		catch(System.Exception ex){
			Debug.Log (ex);

		}

		try{
			Uri requestUri = new Uri("http://dataservice.accuweather.com/currentconditions/v1/"+locationKey+"?apikey=" + BaseWeather.API_KEY);
			string s =  getJsonString(requestUri).Result;
            s = s.Substring(1, s.Length-2 );
            this.currentData = JObject.Parse (s);
			}
		catch(System.Exception ex){
			Debug.Log (ex);
		}

    }


    public int getWeatherIcon (){
		return (int)currentData["WeatherIcon"];
	}

	public int getTemperature(){
		return (int)currentData["Temperature"]["Metric"]["Value"] ;
	}


    public async Task<string> getJsonString(Uri uri) {
        string jsonString = null;
        HttpClient httpClient = new HttpClient();
        jsonString = await httpClient.GetStringAsync(uri);
        httpClient.Dispose();
        return jsonString;
    }


}



public class FiveDaysForecast{

	HttpClient httpClient = new HttpClient();

    private JObject fivedaysData = null;

	public  FiveDaysForecast(int type_location, string TextorIp)
    {
        string locationKey = null;

	    HttpClient httpClient = new HttpClient();
    

        try
        {
		    
			switch (type_location) {
			case BaseWeather.TEXT_LOCATION:
				{
					Uri requestUri = new Uri("http://dataservice.accuweather.com/locations/v1/cities/autocomplete?apikey="+BaseWeather.API_KEY+"&q="+TextorIp);				
					string s =  getJsonString(requestUri).Result;
                    s = s.Substring(1, s.Length-2 );

					JObject locationData = JObject.Parse (s);

					if (s == "[]") 
					{
						Debug.Log("getLocationKey: no Key string from uri.");
						return ;
					}

					locationKey = (string)locationData["Key"];
					break;
				}
			case BaseWeather.IP_LOCATION:
				{
					Uri requestUri = new Uri("http://dataservice.accuweather.com/locations/v1/cities/ipaddress?apikey="+BaseWeather.API_KEY+"&q="+TextorIp);
					string s =  getJsonString(requestUri).Result;

                    JObject locationData = JObject.Parse (s);

					if(locationData["Key"] == null)
						Debug.Log("getLocationKey: no Key string from uri.");

					locationKey = (string)locationData["Key"];
					break;
				}
			default:
				Debug.Log ("getLocationKey: TYPE ERROR!");
				break;
			}
			}
		catch(System.Exception ex){
			Debug.Log (ex);

		}




		try{
			Uri requestUri = new Uri("http://dataservice.accuweather.com/forecasts/v1/daily/5day/"+locationKey+"?apikey="+BaseWeather.API_KEY+"&metric=true");
			string s =  getJsonString(requestUri).Result;

            this.fivedaysData = JObject.Parse (s);
			}
		catch(System.Exception ex){
			Debug.Log (ex);
		}

    }

    public int getDayIcon (int day){
		//day: 0~4
		Debug.Log(day);
		return (int)fivedaysData["DailyForecasts"][day]["Day"]["Icon"];
	}

	public int getNightIcon(int day){
		//day: 0~4
		Debug.Log(day);
		return (int)fivedaysData["DailyForecasts"][day]["Night"]["Icon"];
	}

	public int getMinTemperature(int day){
		//day: 0~4
		Debug.Log(day);
		return (int)fivedaysData["DailyForecasts"][day]["Temperature"]["Minimum"]["Value"] ;
	}

	public int getMaxTemperature(int day){
		//day: 0~4
		Debug.Log(day);
		return (int)fivedaysData["DailyForecasts"][day]["Temperature"]["Maximum"]["Value"] ;
	}

    public async Task<string> getJsonString(Uri uri) {
        string jsonString = null;
        HttpClient httpClient = new HttpClient();
        jsonString = await httpClient.GetStringAsync(uri);
        httpClient.Dispose();
        return jsonString;
    }



}
#endif
