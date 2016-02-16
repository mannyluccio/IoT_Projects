
CREATE EVENT report_giornaliero
  ON SCHEDULE
    EVERY 1 DAY
    STARTS '2015-12-03 00:05:00' ON COMPLETION PRESERVE ENABLE 
  DO 
		INSERT INTO  meteodata.datimeteo_giornalieri (Data,TempMin,TempMax,TempMedia,UmiditàMin,UmiditàMax,UmiditàMedia,PressioneMin,PressioneMax,PressioneMedia,AriaMedia,PioggiaTotale)

		SELECT now(),min(dm.Temperatura),max(dm.Temperatura),ROUND(avg(dm.Temperatura),2),min(dm.Umidità),max(dm.Umidità),ROUND(avg(dm.Umidità),2),min(dm.Pressione),max(dm.Pressione),ROUND(avg(dm.Pressione),2),ROUND(avg(dm.Aria),2),max(dm.Pioggia)
		FROM datimeteo dm
		WHERE  DATE(dm.Datetime) BETWEEN DATE_ADD(CURDATE(), INTERVAL -1 day) AND CURDATE()
        HAVING COUNT(*) = 1;
        


