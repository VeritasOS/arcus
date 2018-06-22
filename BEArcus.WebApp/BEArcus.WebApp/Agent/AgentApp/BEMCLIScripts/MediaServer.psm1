<#/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/#>function GetLocalMediaServer{
 <# 	.Synopsis
	Function for getting LocalMediaServer.        
	.Description
    This function fetches LocalMediaServer from the agent machine. Uses BEMCLI to fetch the details.	 
	Converts the output in json format.        
	.Example
	GetLocalMediaServer 
	
 #>       
	    $fqmn = "MediaServer.psm1:GetLocalMediaServer"

		try
        {                      Get-BEBackupExecServer -Local -ea stop | ConvertTo-Json		
        }
        catch
        {
            $ErrDetail = "Exception occurred while fetching LocalMediaServer." + $_.Exception.ToString()           
        }
}

function GetSuccessfullJobHistoryByDate{
 <# 	.Synopsis
	Function for getting SuccessfullJobHistories.        
	.Description
    This function fetches SuccessfullJobHistories from the agent machine. Uses BEMCLI to fetch the details.
	Currently it returns only the SuccessfullJobHistories after the specified (UTC) time
    Converts the output in json format.     
	.Example
	
	.Parameter FromTime
	This parameter Specifies a DateTime object to filter SuccessfullJobHistories with a start time.	     
    .Parameter ToTime
	This parameter Specifies a DateTime object to filter SuccessfullJobHistories to end time.	
    .Parameter Number
	 This parameter fetches the SuccessfullJobHistories  based on the Number of SuccessfullJobHistories to be fetched.	
	
 #>
        param
        (
            [DateTime]  $FromTime,
            [DateTime]    $ToTime,            [int] $Number          
        )
	    $fqmn = "MediaServer.psm1:GetSuccessfullJobHistoryByDate"

		try
        {         

           if($Number)
           {
                 Get-BEJobHistory -jobtype backup -jobstatus succeeded -ea stop | where {$_.StartTime.ToUniversalTime() -gt $FromTime.ToUniversalTime() -and $_.StartTime.ToUniversalTime() -lt $ToTime.ToUniversalTime() } | Select -First $Number | ConvertTo-Json            
           }
           else
           {          
                 Get-BEJobHistory -jobtype backup -jobstatus succeeded -ea stop | where {$_.StartTime.ToUniversalTime() -gt $FromTime.ToUniversalTime() -and $_.StartTime.ToUniversalTime() -lt $ToTime.ToUniversalTime() } | ConvertTo-Json                      }
        }
        catch
        {
            $ErrDetail = "Exception occurred while fetching SuccessfullJobHistories." + $_.Exception.ToString()
          
        }
}

export-modulemember -function GetLocalMediaServer

export-modulemember -function GetSuccessfullJobHistoryByDate

