<#
/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/
#>     

function GetJobHistory
{
 <#
 	.Synopsis
	Function for getting JobHistroy.        

	.Description
        This function fetches JobHistory from the agent machine. Uses BEMCLI to fetch the details.
	Currently it returns only the JobHistory according to the parameters passed.
	Converts the output in json string and returns the string.        

	.Example
	GetJobHistory -Number 2

	.Parameter Name
	This parameter specifies the Name of the JobHistory to be fetched.	

	.Parameter JobStatus
    This parameter specifies the JobStatus of the JobHistory to be fetched.
	
	.Parameter JobType
	This parameter specifies the JobType of the JobHistory to be fetched.

	.Parameter Number
	This parameter specifies the Number of the JobHistories to be fetched.	

 #>
        param
        (
            [string]  $Name,
            
            [string]  $JobStatus,
            
            [string]  $JobType,   
            
            [int] $Number        
        )

	    $fqmn = "Jobs.psm1:GetJobs"

		try
        {         
      
            if($Name)
            {         
                   Get-BEJobHistory -ea stop | where {$_.Name -eq $Name} | ConvertTo-Json 
            }
            elseif($JobStatus)
            {
                   Get-BEJobHistory -ea stop | where {$_.JobStatus -eq $JobStatus} | ConvertTo-Json
            }
            elseif($JobType)
            {  
                  Get-BEJobHistory -ea stop | where {$_.JobType -eq $JobType} | ConvertTo-Json                
            }
            elseif($Number)
            {
                   Get-BEJobHistory -ea stop | Select -First $Number | ConvertTo-Json             
            }
            else
            {                
                Get-BEJobHistory -ea stop | ConvertTo-Json   
            }         
           
        }
        catch
        {

            $ErrDetail = "Exception occurred while fetching alerts." + $_.Exception.ToString()
           
        }
}

function GetJobHistoryByDate
{
 <#
 	.Synopsis
	Function for getting active alerts.        

	.Description
        This function fetches acitve alerts from the agent machine. Uses BEMCLI to fetch the details.
	Currently it returns only the active alerts after the specified time
    Converts the output in json format.     

	.Example
	

	.Parameter FromTime
	This parameter Specifies a DateTime object to filter active alert with a start time.	
     
    .Parameter ToTime
	This parameter Specifies a DateTime object to filter active alert to end time.	

    .Parameter Number
	    This parameter fetches the Alerts based on the Number of Alerts to be fetched.	
	
 #>
        param
        (
            [DateTime]  $FromTime,

            [DateTime]    $ToTime,

            [int] $Number          
        )

	    $fqmn = "Alerts.psm1:GetAlerts"

		try
        {         

           if($Number)
           {
                 Get-BEJobHistory -ea stop | where {$_.StartTime.ToUniversalTime() -gt $FromTime.ToUniversalTime() -and $_.StartTime.ToUniversalTime() -lt $ToTime.ToUniversalTime() } | Select -First $Number | ConvertTo-Json            
           }
           else
           {          
                Get-BEJobHistory -ea stop | where {$_.StartTime.ToUniversalTime() -gt $FromTime.ToUniversalTime() -and $_.StartTime.ToUniversalTime() -lt $ToTime.ToUniversalTime() } | ConvertTo-Json           
           }
        }
        catch
        {

            $ErrDetail = "Exception occurred while fetching alerts." + $_.Exception.ToString()
          
        }
}

export-modulemember -function GetJobHistory

export-modulemember -function GetJobHistoryByDate