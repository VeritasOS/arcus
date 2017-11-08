<#/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/#>function GetLocalMediaServer{
 <# 	.Synopsis
	Function for getting active alerts.        
	.Description
        This function fetches active alerts from the agent machine. Uses BEMCLI to fetch the details.	 
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
            $ErrDetail = "Exception occurred while fetching alerts." + $_.Exception.ToString()           
        }
}
export-modulemember -function GetLocalMediaServer