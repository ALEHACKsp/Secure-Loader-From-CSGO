 <?php
 $user = $_POST['usr'];
 $token = $_POST['tkn'];
 $dbhost = "localhost"; $dbuser = "root"; $dbpass = ""; $dbname = "loader";
 $dbdsn = "mysql:host=$dbhost;dbname=$dbname";
 $pdosql = new PDO($dbdsn, $dbuser, $dbpass, array(PDO::ATTR_PERSISTENT => true, PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION));
 $sql = "SELECT * FROM token WHERE LOWER(user)=? AND token=?;";
 $query = $pdosql->prepare($sql);
 $query->execute(array($user, $token));
if ($user = $query->fetch()) 
{
if($user['token'] == $token)
{
 if (time() > $user['exp']) { echo "1339"; }  
 else if (time() < $user['exp']) { echo "1337|" . file_get_contents('ciphered.lst'); }
} 
} 
else if($user['token'] != $token){ echo "1338"; }
?>
    