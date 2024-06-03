VAR currentDay = 1
VAR isUSBPlugged = false

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
As a repairer, your role is crucial to the continued operation of the FixFox. 
# END_KNOT
* [>] -> day_1_2

=== day_1_2 ===

Instructions:

1. Read the error codes for each object
2. Access your workshop
3. Replace faulty parts
4. Place the item in the bin.

* [>] -> day_1_3

=== day_1_3 ===
Please respect the daily quotas. Failure to do so will result in your suspension. 

Yours sincerely 

The company

* [>] -> day_1_4

=== day_1_4 ===
Day 1 tasks:

object \#1 - change the screen
* [>] -> day_1_5

=== day_1_5 ===
# END_KNOT
* [>] -> day_2

=== day_2 ===
Welcome to day 2!

* [>] -> day_2_2

=== day_2_2 ===
Day 2 tasks:

object \#1 - replace batteries

* [>] -> day_2_3

=== day_2_3 ===
# END_KNOT
* [>] -> day_3

=== day_3 ===
Welcome to day 3!

* [>] -> day_3_2

=== day_3_2 ===
Day 3 tasks:

object \#1 - replace batteries
object \#2 - change the screen

* [>] -> day_3_3

=== day_3_3 ===
# END_KNOT
* [>] -> day_4

=== day_4 ===
Welcome to day 4!

* [>] -> day_4_2

=== day_4_2 ===
Day 4 tasks:

object \#1 - change the screen
object \#2 - change the screen

* [>] -> day_4_3

=== day_4_3 ===
# END_KNOT
* [>] -> day_10

=== day_10 ===
Welcome to day 10! (pirate batteries)
* [>] -> day_10_2

=== day_10_2 ===
Day 10 tasks:

object \#1 - change the screen
object \#2 - /!\ analysis error
object \#3 - change the screen

* [>] -> day_17

=== day_17 ===
Welcome to day 17! (pirate screen)
* [>] -> day_17_2

=== day_17_2 ===
Day 17 tasks:
object \#1 - change the screen
object \#2 - change the screen
object \#3 - replace batteries
object \#4 - /!\ analysis error

* [>] -> day_20

=== day_20 ===
Welcome to day 20!

* [>] -> day_20_2

=== day_20_2 ===
Dear repairers,

We are delighted to announce that our company has reached a major milestone: the [Group name] investment group has acquired 50% of our shares. This new strategic alliance will enable us to expand our ambitions with FixFox.

Yours sincerely

The Management

* [>] -> day_23

=== day_23 ===
Welcome to day 23!
Remember to follow the instructions.

* [>] -> day_23_2

=== day_23_2 ===
Dear repairers,
We have observed an increase in unauthorised modules, compromising the safety and integrity of our products. A zero-tolerance policy is now in force: any infringement will result in severe measures being taken, without exception.

Yours sincerely

The Management

* [>] -> day_23_3

=== day_23_3 ===
Day 23 tasks:
object \#1 - change the screen
object \#2 - /!\ analysis error
object \#3 - /!\ analysis error
object \#4 - change the screen

* [>] -> day_25

=== day_25 ===
Welcome to day 25!
Remember to follow the instructions.

* [>] -> day_25_2

=== day_25_2 ===
We are the Open Resilience group, and we are contacting you to let you know that we do not accept the new direction of the company. The FixFox was originally about sustainability, community and innovation. We will not tolerate this great object becoming a monopoly solely focused on profit.

* [>] -> day_25_3

=== day_25_3 ===
We repeat to all repairers, custom parts are not "pirates", they simply symbolise our freedom to use the product.

Sincerely, Open Resilience

* [>] -> day_30

=== day_30 ===
Welcome to day 30! (capteurs)
Remember to follow the instructions.

* [>] -> day_30_2

=== day_30_2 ===
Dear repairers,
As part of our ongoing commitment to innovation and safety, we are now integrating data sensors into repaired modules. These sensors are essential for collecting vital information to improve the user experience and ensure product compliance. Your cooperation in installing these sensors on all repaired modules is imperative and will be closely monitored.

Yours sincerely

The Management

* [>] -> day_35

=== day_35 ===
Welcome to day 35!
Remember to follow the instructions.

* [>] -> day_35_2

=== day_35_2 ===
{isUSBPlugged:
(message qui s'affiche si la clé usb est branchée)
        Alert to all employees,

We have discovered that the company is installing bugs in repaired modules under the pretext of improvements and security. This is a flagrant violation of our right to confidentiality and free use of our devices. We cannot allow this secret surveillance to continue. 
    - else:
        FARzkabQvOzSaEa1BZf1v++tuuNa4e+V+Ltoj7tFHxDdD0TXljsxdlyi3rIgITExITjANEr+8ngSsMEhMzMhsx78zYJo+SE5ISrdWI97GI4EKkQRuY29ZSE5IZA/toy+m9x8CK95BqbwmjjAmcMorHO0/0rmYvoE5RzcmynDHddyb03LnSQqVgPveFEhOSHH68EjO4ta17pLE4gU1yE5IbRPITMzIeHOrzfIITkhxWo/BuFYrszMITM5ISExMyF7O4MYFTXOTFwYhel7iXRnyNgf+1LIkGNZGNvDNx0hMTMh9vyTk4eGfVCcl
        }

* [>] -> day_35_3

=== day_35_3 ===
(message qui s'affiche si la clé usb est branchée)
{ isUSBPlugged:
We call on all repairers to refuse to participate in this practice and to join us in our fight for transparency and a return to the company's core values.

Yours sincerely, Open Resilience
    - else:
V+zE2xjkyExMiGhLG52ITEwIaWGNwcDJcj9jz2p67ffi8BDfOH0JSaLjNueWasjdsXI1vlUM235cc0jcp3/KP/fNo2OVq1TuB23ytG8fGRTwqwejSEzMyFEeovDIHRC/hyWHSiqahtV3ZHfHeGzJC4d4/JEXMVDjCiLcA/1RagdPLwpuOjwTSCzxnVOEb
}

* [>] -> day_45
=== day_45 ===
Welcome to day 45!

* [>] -> day_45_2

=== day_45_2 ===
Request to all repairers,

We're getting close. Thanks to your help, the company is teetering on the brink, and more and more of you are getting involved. We have high-placed contacts, enabling us to contact you directly on your terminals. We need you one last time to force the company to honour its commitments: send us your data.

We will be transparent: this action will be traced by the company's security and you risk immediate suspension. Nevertheless, every repairer's data counts, and we ask you, in the name of your ethics, to help us make the company comply.

Sincerely, Open Resilience

* [>] -> day_45_3

=== day_45_3 ===

We will be transparent: this action will be traced by the company's security and you risk immediate suspension. Nevertheless, every repairer's data counts, and we ask you, in the name of your ethics, to help us make the company comply.

Sincerely, Open Resilience

* [Send all repair datas] -> day_55
* [Don't send anything] -> day_55_bis

=== day_55 ===
Welcome to day 46!

* [>] -> day_55_2

=== day_55_2 ===
Information to all repairers,

We have reached our goal. Thanks to your unwavering support, the investors have given up control, and the company has been officially taken over by Open Resilience. Your dedication and efforts have made this possible, and more of you joined our cause each day.

We extend our deepest gratitude for your help. With your support, we have successfully steered the company towards a more transparent and ethical future. This is a victory for all of us who believe in the right to repair, technological independence, and sustainable practices.

* [>] -> day_55_3

=== day_55_3 ===
Thank you for standing with us and for your commitment to making a difference. Together, we will continue to promote these values and build a future where repairability and independence are at the forefront.

Sincerely,
Open Resilience

* [>] -> END

=== day_55_bis ===
Welcome to day 46!

* [>] -> day_55_2_bis

=== day_55_2_bis ===
Dear employees,

We are pleased to announce that we have identified and dismissed the individuals involved in recent unauthorized activities. Your loyalty and hard work have been instrumental in maintaining the company's stability during this time.

* [>] -> day_55_3_bis

=== day_55_3_bis ===
We extend our sincere thanks to all dedicated employees whose efforts have contributed to our continued success. Your commitment to excellence is recognized and appreciated. We encourage you to keep up the great work as we move forward together.

Yours sincerely,

The Management

* [>] -> END

=== generic_day ===
Bienvenue à un nouveau jour !
* [>] -> start


=== tutorial_start ===
Bienvenue dans l'entreprise ! Vous êtes maintenant un réparateur chargé de maintenir nos objets technologiques en parfait état. Votre mission est de diagnostiquer et réparer les objets qui vous sont confiés. Suivez les instructions pour commencer.
* [>] -> tutorial_diagnose

=== tutorial_diagnose ===
Pour commencer, utilisez le terminal pour diagnostiquer l'objet. Cliquez sur le bouton 'Diagnostiquer' pour voir les problèmes de l'objet.
* [Diagnostiquer] -> tutorial_screen_repair

=== tutorial_screen_repair ===
Le diagnostic montre que l'écran de l'objet est endommagé. Remplaçons l'écran.

* [>] -> tutorial_screen_repair_2

=== tutorial_screen_repair_2 ===
#START_TUTO
* [>] -> tutorial_remove_screen

=== tutorial_remove_screen ===
Maintenant, installez le nouvel écran en appuyant un moment sur celui-ci et en le plaçant sur l'objet.
* [Installer l'écran] -> tutorial_remove_screen_2

=== tutorial_remove_screen_2 ===
#ENDKNOT_TUTO
* [-] -> tutorial_finalize

=== tutorial_finalize ===
L'objet est maintenant réparé. Cliquez sur le bac d'envoie pour terminer votre tâche.
* [Placer l'objet] -> tutorial_finalize_2

=== tutorial_finalize_2 ===
#ENDKNOT_TUTO
* [-] -> tutorial_end

=== tutorial_end ===
Félicitations ! Vous avez terminé votre premiere reparation. Votre formation est terminée.
* [Commencer le jeu] -> start
