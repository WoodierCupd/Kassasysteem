use developerland_kassa;

show grants for kassagebruiker;
show grants for kassagebruiker@'%';

select 
	categorie_id,
    naam,
    beschrijving,
    is_actief
from product_categorieen
order by categorie_id;

select 
	product_id, 
    naam, 
    beschrijving, 
    stuksprijs,
    is_actief
from producten
order by product_id;

select
	rol_id,
    naam,
    beschrijving,
    is_actief
from rollen
order by rol_id;

select 
	gebruiker_id,
    rol_id,
    gebruikersnaam,
    volledige_naam,
    pincode,
    wachtwoord,
    is_actief
from gebruikers
order by gebruiker_id;