   M S B A M L   `   `   `  ����M  IPO-FileRetrieverUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:PO_FileRetrieverUIPO_FileRetrieverUI  5   	   #    PO_FileRetrieverUI.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:PO_FileRetrieverUI     5   	     � Title$$  File Retriever Acknowledgement��5   	   $	��320��60   $	��420��6=    � Icon$ N.ico��6I    � 	Resources 5
       �� 
TargetType �� Property �� Value��%w   '��       �� 5   
   " ��6   .�5      ��  � ��$ 65��6)   �� 5       � ��$	 5,3q�6(   5      5      .��5      ��  �� Padding$ 30,15q�6   ._�5   
   �  � RowDefinitions 5      �� 5      $
��auto �6    �� 5      $��* �6    �� 5      $
��auto �6    �� 5      $��* �6    $��20��6+   �� 5      $
��auto �6    �� 5      $��* �6    $��20��6+   �� 5      $
��auto �6    �� 5      $��* �6    .+�5       � $��0��6   .+�5!      �� -   5         ��Name$ btn_language��5!      $��0��6/   $��30��6<   $��Right=�6G      Active_Lang#���   5"      
�������
�������5#      5%      � 5'      $��2��6   .+�5(      �� $-��'/Images/Nita Logo Sentient - Simple.png��6   5)      �� 5+      $��4��6   .+�5,      ��   DisclaimerTitleStr#���  6   $
��Bold"�6H   	 �� Padding$		 0,3q�6Z   $
��12.5(�6h   $��WrapWithOverflow=�6x   �� 5-      $
��Wrap=�6   
 �� TextAlignment$
 Justify=�60     DisclaimerTextStr#���  5.      5/      � 52      $��6��6    � ColumnDefinitions 53      �� 54      $��3* �6'   �� 55      $
��auto �6'   �� 56      $
��0.2* �6'   �� 57      $
��auto �6'   �� 58      $��3* �6'   .+�5;      �� -   5        $ btn_yes��5;   )   $��1��6     	BtnYesStr#���  6:   6`   �� 5<      -   5        $ btn_no��5<   )   $��3��6     BtnNoStr#���  69   6^   5>      5?      5@      5B      