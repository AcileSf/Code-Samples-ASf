   M S B A M L   `   `   `  ���� M  IPO-FileRetrieverUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:PO_FileRetrieverUIPO_FileRetrieverUI  #    PO_FileRetrieverUI.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     2local clr-namespace:PO_FileRetrieverUI     -     � Title$  File Retriever��$	��350��$	��350�� � Icon$ N.ico��.���  � RowDefinitions �� $��1* ��� $��2* ��� $��1* ��� $��2* ��� $��1* ��� $��2* ��� $��1* ��� $��3* ��� $
��2.5* �.+��� -    ��Name$ title0��$)��#Choose type of package to generate:��$��
 30,0,30,0q�$��Bottom=�$
��Bold"��� -   $ title1��$��2��$��
 30,0,30,0q�$��Bottom=�$
��Bold"��� -    ��Name$ 
tbInputNbr��$��3��$��	2,2.5,0,0q�$��
 30,0,30,0q�$	�� 25��$	��Top=�
������ �� -   $��tbPlaceholder��$��3��$�� 35,3.5,30,0q�$	�� 15��$	��Top=�$��Light"�$��Italic$�
������� K� ZIndex$ -1���� $��1��$��
 30,0,30,0q�.+�	� -   $
��rbPO�� 	� 	GroupName$ G1��$��Purchase Order�� d� 	IsChecked$
 TrueY�	� -   $��rbQuote��$ G1��$��Quote���� -   $ title2��$��4��$��Enter Save Location:��$��
 30,0,30,0q�$��Bottom=�$
��Bold"�� $��5�� � ColumnDefinitions �� $��* ��� $��65 ��� $��30 �.+��� -	   $ 	tbSaveLoc��$��	2,2.5,0,0q�$��	 30,0,0,0q�$	�� 25��$	��Top=�
������ $��1,1,0,1q��� -
   $��tbPlaceholder2��$0��*Enter path of ZIP file desired destination��$�� 35,3.5,0,0q�$	�� 15��$	��Top=�$��Light"�$��Italic$�
�������$ -1���� -   	 ��Name$	 	btnBrowse��$��1��$��65��$��25��$	��Top=�$
��Left=�$��Browse���� -   $ tbFileTypes��$��6��$#��Choose types of files to ZIP:��$��	 30,0,0,0q�$��Bottom=�$
��Bold"��� $��7��$	��Top=�$��30,0,0,0q�.+��� -   
 ��Name$
 ChkPDF���� -   $
 ChkDWG���� -   $
 ChkSTP��� $��8�� �� $��* ��� $��70 ��� $��70 ��� $��70 ��� $��* �.+��� -   $
	 btn1��$��1��$��65��$��30��$��Center=�$��Reset���� -   $
	 btn2��$��2��$��65��$��30��$��Center=�$��Confirm���� -   $
	 btn3��$��3��$��65��$��30��$��Center=�$
��Exit��