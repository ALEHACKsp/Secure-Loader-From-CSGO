<?php
$login = $_POST['usr'];
$senha = sha1($_POST['pss']);
$hwid = sha1($_POST['hdi']);
$dbhost = "localhost";
$dbuser = "root";
$dbpass = "";
$dbname = "loader";
$dbdsn = "mysql:host=$dbhost;dbname=$dbname";
$pdosql = new PDO($dbdsn, $dbuser, $dbpass, array(PDO::ATTR_PERSISTENT => true, PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION));
$sql = "SELECT * FROM users WHERE LOWER(login)=? AND senha=?;";
$query = $pdosql->prepare($sql);
$query->execute(array($login, $senha));
if ($user = $query->fetch()) {
    if ($user['hwid'] == "") {
        $q = $pdosql->prepare("UPDATE users SET `hwid` = ?
                WHERE `ID` = ? ;");
        $q->execute(array(sha1($hwid), $user['ID']));
    } 
    elseif ($user['hwid'] == sha1($hwid)) { } 
    else { die('1339'); }
    if ($user['plano'] == "1") {
        $hash = bin2hex(random_bytes(16));
        $sql = "INSERT IGNORE INTO `token` (`user`,`token`,`exp`) VALUES (?,?,?);";
        $dbhost = "localhost"; $dbuser = "root"; $dbpass = ""; $dbname = "loader";
        $dbdsn = "mysql:host=$dbhost;dbname=$dbname";
        $pdosql = new PDO($dbdsn, $dbuser, $dbpass, array(PDO::ATTR_PERSISTENT => true, PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION));
        $query= $pdosql->prepare($sql);
        $exp = time() + 40; 
        $result = $query->execute(array(trim($login),trim($hash),trim($exp)));
        if ($result){
          echo "1337|" . $hash;
        }
        $_POST = array();
        if (time() > $user['vipfim']) { die('1400'); }
    }
    else if ($user['plano'] == "0") {
        echo "1401";
    }
} else { die("1338"); }
