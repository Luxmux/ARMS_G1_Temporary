   M S B A M L   `   `   `  ���� K  GARMS_WPF_G1_Temp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:ARMS_WPF_G1_TempARMS_WPF_G1_Temp      ARMS_WPF_G1_Temp.Logging   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     0localclr-namespace:ARMS_WPF_G1_Temp       � Title$  ARMS WPF Temp - Logging��$	��402��$	��710�� � Icon$ icon.jpg�� � 
ResizeMode$ CanMinimize=� � 	Resources  �� 
TargetType �� Property �� Value �� 
TargetType �� Triggers	 P� Property
 P� Value   RegularTextBox ��Name �� Triggers �� 
TargetName  dayTimeSlider m� 	Placement m� Fill ]� DecreaseRepeatButton J� Opacity ]� IncreaseRepeatButton ]� Thumb J� SnapsToDevicePixels ��Name �� CornerRadius �� RelativeSource �� AncestorType �� StringFormat � LayoutTransform �� Angle��%�  '��       &	  K    &	 �    �� " ��.���  � ��$ 1q���  � �� �#000000��  � �� �#e6e7e4��  � �� �� " ��.R��� $��1q�
���   �#��z��._��� $��Center=�$��Center=� P� 	 �@J�IsMouseOver
 . . ���  � �� �#d0d0d0P� 	 � {�
 .  . ���  � �� �#394969��  � �� �#b9b9b9�� " ��.���  � �� �� " ��.R��� $��Border��
���   �$��1q�._��� $ PART_ContentHost��$��0q� P� 	 � {�
 .  . ��� $ Border�� � �� �#e6e7e4�� $ Border�� � ��
 ���p���  � �� �#494949P� 	 � {�
 . . ��� $ Border�� � ��
 ������� $ Border�� � ��
 �   ���  � ��
 �   ��� " ��.���  � �� �� .R�� .+�m� $��LeftTick��$��0��$��10��$
 Left=�$y�	Collapsed=�# z��m� $��	RightTick��$��10��$ Right=�$y�	Collapsed=�# z��]� $��
PART_Track�� �� $��Slider.DecreaseLarge�����#2259FF$��1q����#2259FF$	 0.7Q� �� $��Slider.IncreaseLarge�����#F5F5F5$��1q����#F5F5F5$ 0Q� n� ���� " n�.���  � �� �� " n�.R���  . .+�� $��42��$��15��.+��� $ 
Background�����#a2a2a2$��1q�$��0��$��2��$ 5,5,5,5��$��42��$��15��
���   �$ 90Q��� $��Center=�$��Center=�
���   �$��9(����� *Value+ �� " ��$ {0:N1}�� �� $ 90Q�.��� $��0,0,-17,-24q�.+��� -    ��Name$ Logging_actions��#��[  $��Calibri)�$��11(�$y�Visible=�$
��Auto=�$��Visible=�$
��Left=�$��
11,113,0,0q�$	��203��$	��Top=�$	��667�� J� RenderTransformOrigin$ 0.398,0.307&��� -    ��Name$ LoggingNewBut��$��Create New Log File��{�. $
��Left=�$��26��$	��123��$��
429,17,0,0q�$	��Top=��� -   $ LoggingExistingBut��$��Select Existing File��{�. $
��Left=�$��26��$	��123��$��
555,17,0,0q�$	��Top=��� -   $ LoggingOnOffBut��$��Start Logging��$
��Left=�$��
429,80,0,0q�$	��Top=�$��26��$ 0.952,-0.051&�$	��249���� -   $ filepath��$��[Please choose log path]��$
��Left=�$��	13,17,0,0q�$	��Top=�$	��412��$ 1.359,-7.032&�$��26���� -   $ 	Close_But��$��Calibri)�$��11(�$��Close��{�. $
��Left=�$��23��$��82��$��596,324,0,0q�$	��Top=��� -    ��Name$ logInterval��$��26��$��Bottom=�$��13,0,460,291q�$ 0.952,-0.051&�.@���   �� 
IsSelected  . .��Log Every 1 Sec�� .��Log Every 5 Sec�� .��Log Every 10 Sec�� .��Log Every 15 Sec�� .��Log Every 30 Sec�� .��Log Every 60 Sec