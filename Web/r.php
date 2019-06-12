
  <?php
    if (isset($_GET['usr'])){
      $sql = "INSERT IGNORE INTO `users` (`login`, `senha`,`vipfim`,`plano`,`config`) VALUES (?,?,?,?);";
      $dbhost = "localhost";
      $dbuser = "root";
      $dbpass = "";
      $dbname = "loader";
      $dbdsn = "mysql:host=$dbhost;dbname=$dbname";
      $pdosql = new PDO($dbdsn, $dbuser, $dbpass, array(PDO::ATTR_PERSISTENT => true, PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION));
      $query= $pdosql->prepare($sql);
      $tempo = time() + (30 * 24 * 60 * 60); //1 mes
      $result = $query->execute(array(trim($_GET['usr']),trim(sha1($_GET['pss'])),$tempo,"1","/Configs/". $_GET['usr'] .".cfg"));
      if ($result){
        echo  "1337";
      }
      $_GET = array();
    }
     ?>
    