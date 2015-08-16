# Grand Maps for Muzei Services

The service powering Grand Maps for Muzei.

# API Reference

### **`GET map`**

#### `featured`

Returns the featured map of the day, with the date and time of the next update.

`GET /v1/maps/featured`

	{
	  "Id": "rq-nUqTQd0-iM1T7Mogc7w",
	  "Title": "The Slave Population of the Southern United States",
	  "Author": "Census",
	  "Year": 1860,
	  "ImageAddress": "https://grandmaps.blob.core.windows.net/images/rYi9PhggNUyLU1Ev1DvFAA.jpg",
	  "ReferenceAddress": "http://www.reddit.com/r/MapPorn/comments/21u6fj/percentage_of_slaves_by_us_county_1860_the_map/",
	  "NextUpdate": 1398214800000
	}


#### `random/{previous:length(22)?}`

Returns a map that avoids selecting the one specified by id in the `previous` parameter.

`GET /v1/maps/random/rq-nUqTQd0-iM1T7Mogc7w`

	{
	  "Id": "-0dwhVtfGEmi5RMX7Vp7Hw",
	  "Title": "Explorations and Surveys of the Expedition",
	  "Author": "Royal Geographic Society",
	  "Year": 1909,
	  "ImageAddress": "https://grandmaps.blob.core.windows.net/images/zNx3tAg1Ckm4jgKGCo4FbA.jpg",
	  "ReferenceAddress": "http://memory.loc.gov/cgi-bin/query/D?gmd:2:./temp/~ammem_NS7P::",
	  "NextUpdate": null
	}


#### `{source:alpha}/{id:length(22)}`

Returns a single map using the keys specified.

`GET /v1/maps/main/rq-nUqTQd0-iM1T7Mogc7w`

	{
	  "Id": "rq-nUqTQd0-iM1T7Mogc7w",
	  "Title": "The Slave Population of the Southern United States",
	  "Author": "Census",
	  "Year": 1860,
	  "ImageAddress": "https://grandmaps.blob.core.windows.net/images/rYi9PhggNUyLU1Ev1DvFAA.jpg",
	  "ReferenceAddress": "http://www.reddit.com/r/MapPorn/comments/21u6fj/percentage_of_slaves_by_us_county_1860_the_map/",
	  "NextUpdate": null
	}
