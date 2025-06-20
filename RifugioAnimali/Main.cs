// SELECT COUNT(adozione_id) from adozione WHERE year(data) = YEAR(CURDATE())GROUP BY MONTH(data) ORDER BY mese;
// SELECT COUNT(adozione_id) from adozione WHERE month(data) = month(CURDATE());


/* SELECT 
    (TotaleAdozioni * 100.0 / TotaleAnimali) AS PercentualeAdozioni
FROM 
    (SELECT COUNT(*) AS TotaleAdozioni FROM adozione) AS A,
    (SELECT COUNT(*) AS TotaleAnimali FROM animale) AS B; */

/* -- Quanto tempo resta mediamente un animale prima di essere adottato?
select sum(to_days(ad.data) - to_days(ing.data)) / count(distinct ad.adozione_id) as permanenza_media
from adozione ad
join animale an on an.animale_id = ad.animale_id
join ingresso ing on ing.animale_id = an.animale_id
where an.animale_id in(
	select animale_id from adozione
)
; */