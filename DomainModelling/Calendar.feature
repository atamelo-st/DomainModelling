Feature: Global Calendar
In order to keep track of events
As a user of the Global Calendar
I want to be able to create, update, remove one-off (regular) and recurring events
As well as modify single occurrences of recurring events independently of the recurring event itself

	Rule: Calendar can show events for the period of time at most 7 days long
		
		Scenario: Read calendar events for the period more that 7 days long
			Given Users specify the following <periodStart> and <periodEnd>:
				| periodStart | periodEnd  |
				| 2020-01-01  | 2020-01-20 |
				| 2019-12-29  | 2020-01-06 |
			When Users try to read the calendar events
			Then Users should get an error


		Scenario: Read calendar events for the period no more that 7 days long
			Given The following one-off events exist:
				| eventTitle | startTime              | endTime                |
				| Event1     | 2020-01-01 07:00:00 AM | 2020-02-01 07:00:00 AM |
				| Event2     | 2020-01-02 09:00:00 PM | 2020-01-02 10:00:00 PM | 
				| Event3     | 2020-08-08 08:00:00 PM | 2020-08-08 09:00:00 PM | 
			And The following recurring events exist:
				| eventTitle | startDate  | endDate | repeatPattern           | startTime | endTime  |
				| Event4     | 2019-01-01 | Never   | Weekly: Tue + Wed + Thu | 09:00 AM  | 10:00 AM |
			And Users specify the following <periodStart> and <periodEnd>:
				| periodStart | periodEnd  |
				| 2020-01-01  | 2020-01-02 |
			Then Users should get in response the following one-off events:
				| eventTitle          | startTime              | endTime                |
				| Event1              | 2020-01-01 07:00:00 AM | 2020-02-01 07:00:00 AM |
				| Event2              | 2020-01-02 09:00:00 PM | 2020-01-02 10:00:00 PM |
			And Users should also get in response the following recurring event occurrences:
				| eventTitle              | date       | startTime | endTime  |
				| Event4 Occurrence n     | 2020-01-01 | 09:00 AM  | 10:00 AM |
				| Event4 Occurrence n + 1 | 2020-01-02 | 09:00 AM  | 10:00 AM |

	
	Rule: When calendar events are read for a specified period, period start and period and are inclusive dates
	#TODO: scenarios


	Rule: Event end time has to be past the event start time

		Scenario Outline: Create a one-off event with start time equal or past the end time
			Given Users specify the start time to be <startTime>
			And the end time to be <endTime>
			When Users try to create the event
			Then Users should get an error

			Examples:
				| startTime              | endTime                |
				| 2021-01-01 08:00:00 AM | 2021-01-01 08:00:00 AM |
				| 2020-08-08 10:00:11 PM | 2019-01-01 07:00:00 AM |


	Rule: Event time is precise down to minutes, seconds do not matter

		Scenario Outline: Create a one-off event with time specified down to seconds
			Given Users specify the <eventTitle> event's start time to be <startTime>
			And The end time to be <endTime>
			When Users have the <eventTitle> event created
			And Users read the calendar's events for the period from <startTime> to <endTime>
			Then Users get back <adjustedStartTime> and <adjustedEndTime> for the event <eventTitle>

			Examples:
				| eventTitle | startTime              | endTime                | adjustedStartTime      | adjustedEndTime        |
				| Event1     | 2020-01-01 06:00 AM    | 2020-01-01 07:00:00 AM | 2020-01-01 06:00:00 AM | 2020-01-01 07:00:00 AM |
				| Event2     | 2021-01-02 08:00:17 AM | 2021-01-02 08:32:01 AM | 2021-01-02 08:00:00 AM | 2021-01-02 08:32:00 AM |



	Rule: Both one-off and recurring events may overlap
	#TODO: Scenarios

	
	Rule: Users can modify details of occurrences of reccurring events independently from the 'parent' recurring event itself

		Scenario: Modify details of an occurrence of a recurring event
			Given The following recurring event exists:
				| eventTitle | startDate  | endDate | repeatPattern           | startTime | endTime  |
				| Event1     | 2021-08-09 | Never   | Weekly: Tue + Wed + Thu | 09:00 AM  | 10:00 AM |
			When Users modify the following occurrences for the following dates to be:
				| occurrenceDate | newTitle           |
				| 2021-08-11     | Event1 - Wednesday |
			And Users modify the following occurrences for the following dates to be:
				| occurrenceDate | newEndTime |
				| 2021-08-12     | 10:30 AM   |
			And Users read the calendar's events for the following period:
				| periodStart | periodEnd  |
				| 2021-08-09  | 2021-08-13 |
			Then Users get back the following events:
				| eventTitle         | date       | startTime | endTime  |
				| Event1             | 2021-08-10 | 09:00 AM  | 10:00 AM |
				| Event1 - Wednesday | 2021-08-11 | 09:00 AM  | 10:00 AM |
				| Event1             | 2021-08-12 | 09:00 AM  | 10:30 AM |