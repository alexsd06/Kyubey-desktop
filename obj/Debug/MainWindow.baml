   M S B A M L   `   `   `  ����C  ?QB桌宠, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:QB_WPFQB_WPF  5   	       QB_WPF.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:QB_WPF     5   	   U QWpfAnimatedGif, Version=2.0.0.0, Culture=neutral, PublicKeyToken=9e7cd3b544a090dc,gif"http://wpfanimatedgif.codeplex.com  5   	   -   5          � Title$  QB��5	   	   $	��200��6   $	��300��6!    � WindowStyle$
 None=�6-    � AllowsTransparency .  � Topmost . ��5
      �� 5   
    �� Opacity$ 0Q�6    �� Color$ White��6&   .��5      � .+�5   
   �� -   5         ��Name$	 img��5      "  WpfAnimatedGif.ImageBehavior   AnimatedSource$ /QB_res/QB.gif��6    � ContextMenu 5      �� 5      .@�5      m� $��About the creators��6   .@�5      m� -   5        $1��+bilibili: haojiezhe12345; website: alexsd06��5   #   6X   m� 5      -   5        $+��%Github Repo: haojiezhe12345; alexsd06��5   #   6R   5      m� 5      -   5        	 m�Name$	 idleList��5      $��Idle Action��61   .@�5      m� 5      m� 5      -   5        $��Resize��5      6/   m� 5      -   5        $��Open Folder (Custom GIFs)��5      6B   m� 5      -   5        $	 chkMute��5      $
��Mute��60   
 m� IsCheckable
 . 6K   m� 5      -	   5        $
��Exit��5      6-   5      5      r� 5   
   -
   5         r�Name$ player��5       r� LoadedBehavior$ Manual=�6'   $y�Hidden=�6^   6r   � 5    
   -   5         �Name$ 
GridResize��5       ���	#55000000$y�Hidden=�6:   5!      5"      