   M S B A M L   `   `   `  ���� U  QLiveCharts.Wpf, Version=0.9.7.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf   ARMS_WPF_G1_Temp%   ARMS_WPF_G1_Temp.Communications   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Dlvc4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf     UtoolkitAhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit       � Title$$  ARMS WPF Temp - Communications�� � SizeToContent$ WidthAndHeight=�$��10(�$	��403��$��815.449�� � Icon$ icon.jpg�����#f5f5f5 � 
ResizeMode$ CanMinimize=� � 	Resources  �� 
TargetType �� Property �� Value �� 
TargetType	 �� Triggers
 P� Property P� Value ��Name �� Triggers �� 
TargetName��%)  '��       '�� K    �� " ��.���  � ��$ 1q���  � �� �#000000��  � �� �#e6e7e4��  � �� �� " ��.R��� $��1q�
���   �#��z��._��� $��Center=�$��Center=�	 P� 
 �@J�IsMouseOver . . ���  � �� �#d0d0d0P� 
 � {� .  . ���  � �� �#394969��  � �� �#b9b9b9�� " ��.���  �@�OverridesDefaultStyle . ��  �@�FocusVisualStyle Z� ��  � �� �� " ��.R��� $��Border��
���   �$��1q�._��� $ PART_ContentHost��$��0q� P� 
 � {� .  . ��� $ Border�� � �� �#e6e7e4�� $ Border�� � �� �#707070��  � �� �#707070P� 
 � {� . . ��� $ Border�� � ��
 ������� $ Border�� � ��
 �   ���  � ��
 �   �.��� $��	1,4,0,-81q� � RowDefinitions �� $
��217* ��� $
��230* � � ColumnDefinitions �� .+��� 
�������$
��Left=�$��97��$��
14,191,0,0q�
�������$	��Top=�$	��277��$��2���� 
�������$
��Left=�$��97��$��	13,76,0,0q�
�������$	��Top=�$	��278���� -    ��Name$ LoggingTextEdit��$y�Visible=�$
��Auto=�$��Visible=�$
��Left=�$��
308,27,0,0q�$	��299��$	��Top=�$	��483��$��2���� ���#f5f5f5$��Serial Configuration��$
��Left=�$��	21,62,0,0q�$	��Top=�$��30��$	��115��$��11(��� ���#f5f5f5$ ��Ethernet TCP Configuration��$
��Left=�$��
21,178,0,0q�$	��Top=�$��28��$	��144��$��11(��� $��COM Type��$
��Left=�$��58,3,0,0q�$	��Top=�$��30��$��72��$��11(��� $��	ModBus ID��$
��Left=�$��	199,3,0,0q�$	��Top=�$��23��$��81��$��11(��� $��COM Port��$
��Left=�$��	24,98,0,0q�$	��Top=�$��24��$��81��$��11(��� -    ��Name$ ComType��$
��Left=�$	��Top=�$	��159�� �� 
IsEditable . $��	14,27,0,0q�$��24��.@��� -    ��Name$ USBType��$��USB (Modbus RTU)��
��������� -   $ 	RS232Type��$��RS-232 (Modbus RTU)��
��������� -   $ EthernetType��$��Ethernet (Modbus TCP)��
��������� -   $��ModbusIDEdit��$��Center=�$��Center=�{�.  $	��255��$��188,27,517,168q�$��11(�$��24���� -   $ 
ComPortBox��$
��Left=�$	��Top=�$	��142�� . $��
133,98,0,0q�$��24��.@��� -   $ Port0��$�� ��
��������� -	   $ Port1��$�� ��
��������� -
   $ Port2��$�� ��
��������� -   $ Port3��$�� ��
��������� -   $ Port4��$�� ��
��������� -   $ Port5��$�� ��
��������� -   $ Port6��$�� ��
��������� $��	COM Speed��$
��Left=�$��
24,136,0,0q�$	��Top=�$��32��$��81��$��11(��� -   $ EthernetIPAddressEdit��$��Center=�$��Center=�{�.  $��255.255.255.255��$��133,212,533,211q�$��11(�$��2���� -   $ EthernetPortEdit��$��Center=�$��Center=�{�.  $	��502��$��133,30,533,176q�$��11(�$��1���� $��
IP Address��$
��Left=�$��
33,210,0,0q�$	��Top=�$��28��$��81��$��11(�$��2���� $
��Port��$
��Left=�$��	33,28,0,0q�$	��Top=�$��28��$��81��$��12(�$��1���� -    ��Name$ AutoscanBut��$��11(�$��Autoscanning...��$��24��$��14,86,612,122q�$��1���� -   $ Manual_ConnectBut��$��11(�$��Manual Connect��$��24��$��202,86,517,122q�$��1��{�.  �� -   $ PauseBut��$��11(�$��Pause��$��24��$��82��$��308,118,418,90q�$��1��{�.  �� -   $ ClipboardBut��$��11(�$��Save to Clipboard��$��24��$��400,118,304,90q�$��1��{�.  �� -   $ ClearBut��$��11(�$��Clear��$��24��$��82��$��516,118,210,90q�$��1��{�.  �� -   $ ComSpeedEdit��$��Center=�$��Center=�{�.  $��115200��$��133,139,533,54q�$��11(��� -   $ 	Close_But��$��Calibri)�$��11(�$��Close��{�. $
��Left=�$��24��$��82��$��709,118,0,0q�$	��Top=�$��1��