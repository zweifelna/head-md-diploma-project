VAR currentDay = 1

-> start

=== start ===
{
    - currentDay == 1: -> day_1
    - currentDay == 2: -> day_2
    - currentDay == 3: -> day_3
    - else: -> generic_day
}

=== day_1 ===
Bienvenue au premier jour !
* [>] -> continue_story
* [Terminer le jeu] -> END
* [Autre] -> END

=== day_2 ===
Bienvenue au deuxième jour !
* [>] -> continue_story

=== day_3 ===
Bienvenue au troisième jour !
* [>] -> continue_story

=== generic_day ===
Bienvenue à un nouveau jour !
* [>] -> continue_story

=== continue_story ===
Vous vous tenez devant une porte. Que faites-vous ?
* [Ouvrir la porte] -> open_door
* [Regarder autour] -> look_around

=== open_door ===
La porte s'ouvre avec un grincement.
# END_KNOT
-> END

=== look_around ===
Vous ne voyez rien d'intéressant.
-> continue_story