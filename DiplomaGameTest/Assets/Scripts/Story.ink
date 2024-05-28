VAR currentDay = 1
VAR current_score = 0

-> start

=== start ===
{
    - currentDay == 1: -> day_1_1
    - currentDay == 2: -> day_2
    - currentDay == 3: -> day_3
    - currentDay == 4: -> day_4
    - else: -> generic_day
}

=== day_1_1 ===
Welcome to the company.

* [>] -> day_1_2

=== day_1_2 ===
Day 1 tasks:
object n1 - change the screen
object n2 - change the screen
* [>] -> day_1_3

=== day_1_3 ===
End
# END_KNOT
* [>] -> day_2

=== day_2 ===
Welcome to day 2!

* [>] -> day_2_2

=== day_2_2 ===
Day 2 tasks:
object n1 - change the screen
object n2 - change the screen
object n3 - change the screen

* [>] -> day_2_3

=== day_2_3 ===
End
# END_KNOT
* [>] -> day_3

=== day_3 ===
Welcome to day 3!

* [>] -> day_3_2

=== day_3_2 ===
Day 3 tasks:
object n1 - replace batteries
object n2 - change the screen
object n3 - change the screen

* [>] -> day_3_3

=== day_3_3 ===
End
# END_KNOT
* [>] -> day_4

=== day_4 ===
Welcome to day 4!

* [>] -> day_4_2

=== day_4_2 ===
Day 4 tasks:
object n1 - change the screen
object n2 - replace batteries
object n3 - change the screen

* [>] -> day_4_3

=== day_4_3 ===
End
# END_KNOT
* [>] -> day_20

=== day_20 ===
Welcome to day 20!

* [>] -> day_20_2

=== day_20_2 ===
Dear repairers,

We are delighted to announce that our company has reached a major milestone: the [Group name] investment group has acquired 50% of our shares. This new strategic alliance will enable us to expand our ambitions with [name of object].

Yours sincerely

The Management

* [>] -> day_25

=== day_25 ===
Welcome to day 25! (paper)

* [>] -> day_25_2

=== day_25_2 ===
Welcome to day 25! (paper)

* [>] -> day_35

=== day_35 ===
Welcome to day 35! (usb key)

* [>] -> day_35_2

=== day_35_2 ===
Welcome to day 35! (usb key)

* [>] -> day_45

=== day_45 ===
Welcome to day 45! (clear terminal)

* [>] -> day_45_2

=== day_45_2 ===
Welcome to day 45! (clear terminal)

* [>] -> day_55

=== day_55 ===
Welcome to day 55! (choice)

* [>] -> day_55_2

=== day_55_2 ===
Welcome to day 55! (choice)

* [>] -> END

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

== game_over ==
Vous avez perdu! Votre score était de {current_score}
* [>] -> END

== quota_achieved ==
Bravo! Vous avez atteint votre quota avec un score de {current_score}.
* [>] -> day_1_1
