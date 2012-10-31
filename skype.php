<?php
	// (location)/Skype Resolve.exe <skype username> <wait (ms)> [<directory>]
	$result = shell_exec('"Skype Resolve.exe" ' . $_GET['user'] . ' 2000');
	
	echo $result;
?>