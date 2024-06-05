VAR currentDay = 1
VAR isUSBPlugged = false
VAR skipTutorial = false
EXTERNAL EndTutorial()

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
Welcome to your first day of work 
As a repairer, your role is crucial to the continued operation of the FixFox.   
* [>] -> day_1_2

=== day_1_2 ===

Instructions:

1. Read the diagnosis on the terminal
3. Replace faulty parts
4. Place the item in the bin.

* [>] -> day_1_3

=== day_1_3 ===
Please respect the daily quotas to avoid suspension. 

Yours sincerely 

The company

* [>] -> day_1_4

=== day_1_4 ===
Day 1 tasks:

object \#1 - change the screen
object \#2 - change the screen
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
object \#2 - change the screen

* [>] -> day_2_3

=== day_2_3 ===
# END_KNOT
* [>] -> day_3

=== day_3 ===
Welcome to day 3!

* [>] -> day_3_2

=== day_3_2 ===
Day 3 tasks:

object \#1 - change the screen
object \#2 - replace batteries
object \#3 - change the screen

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
object \#2 - replace batteries
object \#3 - replace batteries

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
Dear employees,

We are delighted to announce that the [Group name] investment group has acquired 50% of our shares. This strategic alliance will enable us to expand our ambitions with FixFox.

Your hard work has been essential in achieving this milestone, positioning us at the forefront of the industry. Our success reflects the commitment and excellence of our repair team.

* [>] -> day_20_3

=== day_20_3 ===

As we grow, we remain committed to our core values of quality and innovation. Together, we will continue to set new standards in the repair industry.

Yours sincerely,

The Management

* [>] -> day_23

=== day_23 ===
Welcome to day 23!
Remember to follow the instructions.

* [>] -> day_23_2

=== day_23_2 ===
Dear employees,
We have observed an increase in unauthorised components, compromising the safety and integrity of our products. A zero-tolerance policy is now in force: any infringement will result in severe measures being taken, without exception.

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

* [>] -> day_25_2

=== day_25_2 ===
Fellow Repairers,

We are the Open Resilience group, and we are contacting you to let you know that we do not accept the new direction of the company. The FixFox was originally about sustainability, community and innovation. We will not tolerate this great object becoming a monopoly solely focused on profit.

* [>] -> day_25_3

=== day_25_3 ===
We want to be clear, custom components are not "pirates", they simply symbolise our freedom to use the product.

Open Resilience

* [>] -> day_28

=== day_28 ===
Dear employees,

We have noticed some repairers are not complying with the latest protocols. We remind you that adherence to these protocols is mandatory. Failure to comply will result in disciplinary action.

* [>] -> day_28_2

=== day_28_2 ===
Day 28 tasks:
object \#1 - change the screen
object \#2 - replace batteries
object \#3 - /!\ analysis error
object \#4 - change the screen

* [>] -> day_30

=== day_30 ===
Welcome to day 30! (capteurs)
Remember to follow the instructions.

* [>] -> day_30_2

=== day_30_2 ===
Dear employees,
As part of our ongoing commitment to innovation and safety, we are now integrating data sensors into repaired modules. These sensors are essential for collecting vital information to improve the user experience and ensure product compliance. Your cooperation in installing these sensors on all repaired modules is imperative and will be closely monitored.

Yours sincerely

The Management

* [>] -> day_33

=== day_33 ===
Dear employees,

Recent assessments show that some repairers are still using unauthorised components. We stress the importance of following company-approved guidelines. Non-compliance will not be tolerated.

* [>] -> day_33_2

=== day_33_2 ===
Day 33 tasks:
object \#1 - replace batteries
object \#2 - change the screen
object \#3 - /!\ analysis error
object \#4 - replace batteries

* [>] -> day_35

=== day_35 ===
Welcome to day 35!
Remember to follow the instructions.

* [>] -> day_35_2

=== day_35_2 ===
{isUSBPlugged:
Fellow Repairers,

We have discovered that the company is installing bugs in repaired modules under the pretext of improvements and security. This is a flagrant violation of our right to confidentiality and free use of our devices. We cannot allow this secret surveillance to continue. 
    - else:
        FARzkabQvOzSaEa 1BZf1v++ tuuNa4e+V+Ltoj7 tFHxDdD0TXljsxdl yi3rIgITExI TjANEr+8ngS sMEh MzMhsx78 zYJo+SE5I SrdWI97GI 4EKkQRuY29ZSE5IZA/t oy+m9x8CK90/0rmYvoE5Rz cmynDHddyb03LnS QqVgPveFEhO
        SHH68 EjO4ta 17pLE4gU 1yE5IbRPITM zIeHOr zfIITkh xWo/Bu FYrszMITM5 ISExMy F7O4MYFTXO TFwYhe l7iXR nyNgf+1LIk GNZGNvDNx0hM TMh 9vyTk4e GfVC cl
        }

* [>] -> day_35_3

=== day_35_3 ===
(message qui s'affiche si la clé usb est branchée)
{ isUSBPlugged:
We call on everyone to refuse to participate in this practice and to join us in our fight for transparency and a return to the company's core values.

Open Resilience
    - else:
V+zE2xjkyExMiGhLG52ITEwIaWGNwcDJcj9jz2p67ffi8BDfOH0JSaLjNueWasjdsXI1vlUM235cc0jcp3/KP/fNo2OVq1TuB23ytG8fGRTwqwejSEzMyFEeovDIHRC/hyWHSiqahtV3ZHfHeGzJC4d4/JEXMVDjCiLcA/1RagdPLwpuOjwTSCzxnVOEb
}

* [>] -> day_40

=== day_40 ===
Welcome to day 40!

* [>] -> day_40_2

=== day_40_2 ===
Dear Repairers,

We at Open Resilience need your immediate assistance to combat the ongoing threats posed by FixGames. To safeguard our mission, we must take drastic measures.

* [>] -> day_40_3

=== day_40_3 ===
We require you to force updates on all devices you repair. These updates, although labeled as "pirate," are crucial to maintain our control and ensure the success of our cause.

Please proceed with these updates on every device you handle.

Open Resilience

* [>] -> day_45

=== day_45 ===
Welcome to day 45!

* [>] -> day_45_2

=== day_45_2 ===
Fellow Repairers,

We're getting close. Thanks to your help, the company is teetering on the brink, and more and more of you are getting involved. We have high-placed contacts, enabling us to contact you directly on your terminals. We need you one last time to force the company to honour its commitments: send us your data.

* [>] -> day_45_3

=== day_45_3 ===

We will be transparent: this action will be traced by the company's security and you risk immediate suspension. Nevertheless, every repairer's data counts, and we ask you, in the name of your ethics, to help us make the company comply.

Open Resilience

* [Send all repair datas] -> day_55
* [Don't send anything] -> day_55_bis

=== day_55 ===
Welcome to day 46!

* [>] -> day_55_2

=== day_55_2 ===
Fellow Repairers,

We have reached our goal. Thanks to your unwavering support, the investors have given up control, and the company has been officially taken over by Open Resilience. Your dedication and efforts have made this possible, and more of you joined our cause each day.

We extend our deepest gratitude for your help. With your support, we have successfully steered the company towards a more transparent and ethical future. This is a victory for all of us who believe in the right to repair, technological independence, and sustainable practices.

* [>] -> day_55_3

=== day_55_3 ===
Thank you for standing with us and for your commitment to making a difference. Together, we will continue to promote these values and build a future where repairability and independence are at the forefront.

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
Welcome to a new day!
* [>] -> start


=== tutorial_start ===
Welcome to the company! You are now a repairer responsible for keeping our technological objects in perfect condition. Your job is to diagnose and repair the objects entrusted to you. Follow the instructions to get started.
* [Tutorial] -> tutorial_diagnose
* [Skip tutorial] 
    ~ EndTutorial()
    -> start

=== tutorial_diagnose ===
To begin, use the terminal to diagnose the object. Click on the ‘Diagnose’ button to see the object's problems.
* [Diagnose] -> tutorial_screen_repair

=== tutorial_screen_repair ===
The diagnosis shows that the object's screen is damaged. Let's replace the screen.
* [>] -> tutorial_screen_repair_2

=== tutorial_screen_repair_2 ===
#START_TUTO
* [>] -> tutorial_remove_screen

=== tutorial_remove_screen ===
Now install the new screen by pressing on it for a moment and placing it on the object.
* [Install the screen] -> tutorial_remove_screen_2

=== tutorial_remove_screen_2 ===
#ENDKNOT_TUTO
* [-] -> tutorial_finalize

=== tutorial_finalize ===
The object is now repaired. Click on the send tray to finish your job.
* [Place object] -> tutorial_finalize_2

=== tutorial_finalize_2 ===
#ENDKNOT_TUTO
* [-] -> tutorial_end

=== tutorial_end ===
Congratulations! You have completed your first repair. Your training is complete.
* [Start the game] -> start
