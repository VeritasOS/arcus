<#
/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/
#>     

function GetJobs
{
 <#
 	.Synopsis
	Function for getting Jobs.        

	.Description
        This function fetches Job from the agent machine. Uses BEMCLI to fetch the details.
	Currently it returns only the Jobs according to the parameters passed.
	Converts the output in json string and returns the string.        

	.Example
	GetJobs -Number 2

	.Parameter Name
	This parameter specifies the Name of the Job to be fetched.	

	.Parameter JobType
    This parameter specifies the JobType of the Job to be fetched.
	
	.Parameter TaskType
	This parameter specifies the TaskType of the Job to be fetched.

	.Parameter Number
	This parameter specifies the Number of the Jobs to be fetched.	

 #>
        param
        (
            [string]  $Name,
            
            [string]  $JobType,
            
            [string]  $TaskType,   
            
            [int] $Number           
        )

	    $fqmn = "Jobs.psm1:GetJobs"

		try
        {
 
            $LocalServer = Get-BEBackupExecServer -Local -ea stop
       
            if($Name)
            {
                   Get-BEJob -ea stop | where {$_.Name -eq $Name} | ConvertTo-Json 
            }
            elseif($JobType)
            {        
                   Get-BEJob -ea stop | where {$_.JobType -eq $JobType} | ConvertTo-Json 
            }
            elseif($TaskType)
            {  
                  Get-BEJob -ea stop | where {$_.TaskType -eq $TaskType} | ConvertTo-Json              
            }
            elseif($Number)
            {
                   Get-BEJob -ea stop | Select -First $Number | ConvertTo-Json               
            }
            else
            {                
                Get-BEJob -ea stop | ConvertTo-Json 
            }       
          
        }
        catch
        {
            $ErrDetail = "Exception occurred while fetching alerts." + $_.Exception.ToString()          
        }
}

export-modulemember -function GetJobs



