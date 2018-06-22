<#
/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/
#>
function GetAlerts
{
 <#
 	.Synopsis
	Function for getting active alerts.        

	.Description
        This function fetches active alerts from the agent machine. Uses BEMCLI to fetch the details.	 
	Converts the output in json format.        

	.Example
	GetAlerts -Number 2

	.Parameter Name
        This parameter fetches the Alerts based on the Alert Name.	

	.Parameter Severity
	    This parameter fetches the Alerts based on the Severity.

	.Parameter Category
	    This parameter fetches the Alerts based on the Category.

	.Parameter Number
	    This parameter fetches the Alerts based on the Number of Alerts to be fetched.
 #>
        param
        (
            [string]  $Name,
            
            [string]  $Severity,
            
            [string]  $Category,   
            
            [int] $Number             

        )

	    $fqmn = "Alerts.psm1:GetAlerts"

		try
        {      
            $LocalServer = Get-BEBackupExecServer -Local -ea stop
      
            if($Name)
            { 
                   Get-BEAlert -ea stop | where {$_.Name -eq $Name -and $_.BackupExecServerName -eq $LocalServer} | ConvertTo-Json 
            }
            elseif($Severity)
            {        
                   Get-BEAlert -ea stop | where {$_.Severity -eq $Severity -and $_.BackupExecServerName -eq $LocalServer} | ConvertTo-Json 
            }
            elseif($Category)
            {  
                  Get-BEAlert -ea stop | where {$_.Category -eq $Category -and $_.BackupExecServerName -eq $LocalServer} | ConvertTo-Json                 
            }
            elseif($Number)
            {
                   Get-BEAlert -ea stop | where {$_.BackupExecServerName -eq $LocalServer} | Select -First $Number | ConvertTo-Json             
            }
            else
            {                
                Get-BEAlert -ea stop | where {$_.BackupExecServerName -eq $LocalServer} | ConvertTo-Json  
            }                    
			
        }
        catch
        {

            $ErrDetail = "Exception occurred while fetching alerts." + $_.Exception.ToString()
           
        }
}

function GetAlertsByDate
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
           
           $LocalServer = Get-BEBackupExecServer -Local -ea stop

           if($Number)
           {
                 Get-BEAlert -ea stop | where {$_.Date.ToUniversalTime() -gt $FromTime.ToUniversalTime() -and $_.Date.ToUniversalTime() -lt $ToTime.ToUniversalTime() -and $_.BackupExecServerName -eq $LocalServer} | Select -First $Number | ConvertTo-Json            
           }          
           else
           {          
                Get-BEAlert -ea stop | where {$_.Date.ToUniversalTime() -gt $FromTime.ToUniversalTime() -and $_.Date.ToUniversalTime() -lt $ToTime.ToUniversalTime() -and $_.BackupExecServerName -eq $LocalServer} | ConvertTo-Json       
           }
        }
        catch
        {

            $ErrDetail = "Exception occurred while fetching alerts." + $_.Exception.ToString()
          
        }
}

export-modulemember -function GetAlertsByDate


export-modulemember -function GetAlerts

