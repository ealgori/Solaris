using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using CommonFunctions.Extentions;
using System.IO;
using System.Runtime.InteropServices;
using CommonFunctions;
//using MailProcessing;
using Redemption;
using System.Reflection;
namespace OHDBServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Clear();
            var pict = GetPicture();
            Console.WriteLine(pict);

            //var hHeap = Heap.HeapCreate(Heap.HeapFlags.HEAP_GENERATE_EXCEPTIONS, 0, 0);
            //// if the FriendlyName is "heap.vshost.exe" then it's using the VS Hosting Process and not "Heap.Exe"
            //Console.WriteLine(AppDomain.CurrentDomain.FriendlyName + " heap created");
            //uint nSize = 100 * 1024 * 1024;
            //ulong nTot = 0;
           
            //    for (int i = 0; i < 1000; i++)
            //    {
            //        try
            //        {
            //            var ptr = Heap.HeapAlloc(hHeap, 0, nSize);
            //            nTot += nSize;
                      

            //        }
            //        catch (Exception ex)
            //        {
                       
            //            Console.WriteLine(String.Format("Max heap size: {0} ", nTot.ToFileSize() ));
            //            break;
            //        }
            //    }
            
           

            //Heap.HeapDestroy(hHeap);
         
         

            Console.WriteLine(System.Environment.NewLine+System.Environment.NewLine);
            Uri WsBaseAddress = new Uri("https://RU00112284:8080/Service/ws");
            Uri baseAddress = new Uri("http://RU00112284:8082/Service/service1");
            // Create the ServiceHost.
            using (ServiceHost host = new ServiceHost(typeof(Service.Service1), baseAddress))
            {
                // Enable metadata publishing.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);
                
                BasicHttpBinding binding = new BasicHttpBinding();


               // host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true /*Do not specify URL at all*/});
                host.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
                host.AddServiceEndpoint(typeof(Service.IService1), binding, string.Empty /*Url here can either be empty or the same one as serviceUri*/);


                // Open the ServiceHost to start listening for messages. Since
                // no endpoints are explicitly configured, the runtime will create
                // one endpoint per base address for each service contract implemented
                // by the service.
                host.Open();
                


                Console.WriteLine("The service is ready at {0}", baseAddress);
                //http://RU00112284:8080/Service
                foreach (var endp in host.Description.Endpoints)
                {
                    Console.WriteLine(endp.Address+", "+endp.Binding.Name+", "+endp.Name);

                }

                Console.WriteLine("Press <Enter> to stop the service.");
                OHDBServiceProxy.Service1 proxy = new OHDBServiceProxy.Service1();
                proxy.GetStatus();
                Console.WriteLine(proxy.GetStatus());
                string val = "";
                while(val!="exit")
                {
                     val = Console.ReadLine();
                }
               
               
                
                // Close the ServiceHost.
                host.Close();
            }


           
        }
     
        private static string GetPicture()
        {
            List<string> pictures = new List<string>();
            pictures.Add(@"                  o
                  |
                ,'~'.
               /     \
              |   ____|_
              |  '___,,_'         .----------------.
              |  ||(o |o)|       ( KILL ALL HUMANS! )
              |   -------         ,----------------'
              |  _____|         -'
              \  '####,
               -------
             /________\
           (  )        |)
           '_ ' ,------|\         _
          /_ /  |      |_\        ||
         /_ /|  |     o| _\      _|| 
        /_ / |  |      |\ _\____//' |
       (  (  |  |      | (_,_,_,____/
        \ _\ |   ------|        
         \ _\|_________|
          \ _\ \__\\__\
          |__| |__||__|
       ||/__/  |__||__|
               |__||__|
               |__||__|
               /__)/__)
              /__//__/
             /__//__/
            /__//__/.
          .'    '.   '.
         (_kOs____)____)");



            pictures.Add(@" _________________________________
    |.--------_--_------------_--__--.|
    ||    /\ |_)|_)|   /\ | |(_ |_   ||
    ;;`,_/``\|__|__|__/``\|_| _)|__ ,:|
   ((_(-,-----------.-.----------.-.)`)
    \__ )        ,'     `.        \ _/
    :  :        |_________|       :  :
    |-'|       ,'-.-.--.-.`.      |`-|
    |_.|      (( (*  )(*  )))     |._|
    |  |       `.-`-'--`-'.'      |  |
    |-'|        | ,-.-.-. |       |._|
    |  |        |(|-|-|-|)|       |  |
    :,':        |_`-'-'-'_|       ;`.;
     \  \     ,'           `.    /._/
      \/ `._ /_______________\_,'  /
       \  / :   ___________   : \,'
        `.| |  |           |  |,'
          `.|  |           |  |
            |  | SSt       |  |");


            pictures.Add(@"   ,'``.._   ,'``.
              :,--._:)\,:,._,.:       All Glory to
              :`--,''   :`...';\      the HYPNO TOAD!
               `,'       `---'  `.
               /                 :
              /                   \
            ,'                     :\.___,-.
           `...,---'``````-..._    |:       \
             (                 )   ;:    )   \  _,-.
              `.              (   //          `'    \
               :               `.//  )      )     , ;
             ,-|`.            _,'/       )    ) ,' ,'
            (  :`.`-..____..=:.-':     .     _,' ,'
             `,'\ ``--....-)='    `._,  \  ,') _ '``._
          _.-/ _ `.       (_)      /     )' ; / \ \`-.'
         `--(   `-:`.     `' ___..'  _,-'   |/   `.)
             `-. `.`.``-----``--,  .'
               |/`.\`'        ,','); SSt
                   `         (/  (/");

            pictures.Add(@"                                __....-------....__
                          ..--'""                   ""`-..
                       .'""                              `.
                     :'                                   `,
                   .'                                       "".
                  :                                           :
                 :                                             b
                d                                              `b
                :                                               :
                :                                               b
               :                                                q
               :                                                `:
              :                                                  :
             ,'                                                  :
            :    _____                  _____                   p'
            \,.-'     `-.            .-'     `-.                :
            .'           `.        .'           `.              :
           /               \      /               \            p'
          :      @          ;    :      @          ;           :
          |                 |    |                 |           :
          :                 ;    :                 ;          ,:
           \               /      \               /           p
           /`.           .'        `.           .'           :
          q_  `-._____.-.            `-._____.-'             :
           /""-__     .""""           ""-.__                    :'
          (_    """"-.'                   """"""---bmw           :
            ""._.-""""                                        ,:
           ,""""                                             P
         .""                                                :
        ""      _.""      .""        .""        _...           :
       P     .""        ""        .'        ,""####)          :
      :     .""       .""        /        ,'######'          :
      :     :       (        ,""        ,########:         ,:
       q    `.      '.       ,        :######,-'          :
       `:    b       q       :        '--''""""             :
        :     :      :       :        :                   :
        :     :      `:      `.       "".                 :'
        q_    :       :       :         )                :
          """"'b`._   ,.`.____,' `._   _.'                 ,
             |.__""""""              """"""     _______.......',
           ,'    """"""""""""""-----.------""""""""""""""               :
           :                 :                            :
           :                 :                            :
           :.__              :           ________.......,'
               """"""""""""""""------'------""""""""""""");

            pictures.Add(@"
       .  .""|
      /| /  |  _.----._ 
     . |/  |.-""        "".  /|
    /                    \/ |__
   |           _.-""""""/        /
   |       _.-""     /.""|     /
    "".__.-""         ""  |     \
       |              |       |
       /_      _.._   | ___  /
     .""  """"-.-""    "". |/.-.\/
    |    0  |    0  |     / |
    \      /\_     _/    ""_/ 
     ""._ _/   ""---""       |  
     /""""""                 |  
     \__.--                |_ 
       )          .        | "". 
      /        _.-""\        |  "".
     /     _.-""             |    "".  
    (_ _.-|                  |     |""-._.
      ""    ""--.             .J     _.-'
              /\        _.-"" | _.-'
             /  \__..--""   _.-'
            /   |      _.-'         
           /| /\|  _.-'                     
          / |/ _.-'                      Silver Saks        
         /|_.-'                                   
       _.-'
 
");
            pictures.Add(@"       _____
        ,-:::::::.
     _,:::::::::::\
    '"";-'::::''::::L
     /:::::/    \::T
    J:::::J      \:J    ____
    |:::::|       LJ ,-::::::-.
    L:::::L      _##/:::::::::\
   /:::::/    ,-::""""-::::::::\
_,::::::/     |:::`:::--:_::::L    ,----------.
`-:::::'      J::::)`,,,_J::::)    |   WELL!  |
               \::(  ( @ ""L::/     | What did |
                (_`   `--'':(      |   you    |
                  \    _,._|`     /   expect? |
                   L  <--,-'    -'-----------'
                  J    (
              __,'     \---...
          ,-""""                \
         J                 ,   \
         L       ,     \    \   \
        J     L  L          J\   \
        L    J  J            L\   \
       J    JL  L            J \   \
       L    LJ  J      (o)   /  \   L
      /    /  \  `._      _,'L   `. J
     J   ,'    \    """"""""""""  /     J  L
    J   J       \      J   /       \ J
    L_  L        )     L  /         L L
   Joo:J        /     c  '\         J  \
   T\::L       /#--[M-K]--#\       ,',_ J
   L/:/       /#############L      `-/|-'
  )   `.    ,'##############J      ,'|\|
 ',.,.''   /#################L    / '\ '.
          J#########,########J    L ,.   L
          L#########L#########L   J_LT   J
         J#########JJ#########T      J__,'");


            pictures.Add(@"               .-.
                     (   )
                      '-'
                      J L
                      | |
                     J   L
                     |   |
                    J     L
                  .-'.___.'-.
                 /___________\
            _.-""'           `bmw._
          .'                       `.
        J                            `.
       F                               L
      J                                 J
     J                                  `
     |                                   L
     |                                   |
     |                                   |
     |                                   J
     |                                    L
     |                                    |
     |             ,.___          ___....--._
     |           ,'     `""""""""'           `-._
     |          J           _____________________`-.
     |         F         .-'   `-88888-'    `Y8888b.`.
     |         |       .'         `P'         `88888b \
     |         |      J       #     L      #    q8888b L
     |         |      |             |           )8888D )
     |         J      \             J           d8888P P
     |          L      `.         .b.         ,88888P /
     |           `.      `-.___,o88888o.___,o88888P'.'
     |             `-.__________________________..-'
     |                                    |
     |         .-----.........____________J
     |       .' |       |      |       |
     |      J---|-----..|...___|_______|
     |      |   |       |      |       |
     |      Y---|-----..|...___|_______|
     |       `. |       |      |       |
     |         `'-------:....__|______.J
     |                                  |
      L___                              |
          """"""----...______________....--'
");


            pictures.Add(@"
                  '#%n.                                
            ..++!%+:/X!!?:              .....          
              ""X!!!!!!!!!!?:       .+?!!!!!!!!!?h.     
              X!!!!!!!!!!!!!k    n!!!!!!!!!!!!!!!!?:   
             !!!!!!!!!!!!!!!!tMMMM!!!!!!!!!!!!!!!!!!?: 
            X!!!!!!!!!!!!!!!!!5MMM``""""%X!!!!!!!!!!!T4XL
            !!!!!!!!!X*"")!!!!!!!?L       %X!!!!!!!!!k `
           '!!!!!!X!`   !!XX!!!!!!L       'X!!!!!!!!!> 
           'X!!!!M`    '"" X!!!!!!!X        !!!!!!!!!!X 
            X!!!f^~('>   X!!!!!!!!!        '!!!!!!!!!X 
            X!!X     `.  X!!!!!!!!f        '!!!!!!!!!! 
            !Xf  O    '  '!!!!!!X""         '!!!!!!!!!X 
           ""`f-.     :~  MX*t!X""           X!!!!!!!!!> 
             >.         W! ~!             '!!!!!!!!!X  
            :`             ':             X!!!!!!!!!!  
            ~ ^`          '              '!!!!!!!!!X   
            `~~~~~~~~     !              X!!!!!!!!!X   
                `~~~      !             '!!!!!!!!!!>   
                  >       !             (!!!!!!!!!X    
                          !             !!!!!!!!!!X    
                  >       '             '!!!!!!!!!!    
                .:         /:`^-.        X!!!!!!!!!>   
           /`  ~/         /(     `:       X!!!X!!!!!:  
         /   : f         f'        !       `XX!M!!!!!% 
        ~   / ~        :  >         4            ""*!!!*
       ~   ~ '        ~   >          :                 
     :   :   !  >  .~     >          '                 
    /   /     ^~~""           .        ~                
   ~   '                  `   \        !               
 :     `                    ~-~""        `              
        \                     > '(       '             
!  ..:)./':                  ~   ':       ~            
>.!!!!!!X   :              :       \~      >           
\!!T!!!!!!                :       /                    
!?X$B!!!!!L ~                    ~        ~            
?!!$$$X!!!X/             >     :        :              
 `!!$$$!X 'N       f    ^-.   ~       .~               
  '!!!!f` 'MMRn.    .-^""   ``       :`                 
    ~!X> ?RMMMMMMMR> .e*~        >`                    
      >`.XMMMMMMMMMMM~ ..    :~                        
      ds@MMMMMMMMMMMMNRMMMM5`                          
      RMMMMMMMMMMMMMMMMMMMMML                          
     'MMMMMMMMMMMMMMMMMMMMMMM                          
     9MMMMMMNMMMMMMMMMMMMMMMMK                         
     MMMMMMMMR8MMMMMMMMMMMMMMM                         
     RMMMMMMMMM$MMMMMMMMMMMMMMk                        
    'MMMMMMMMMMM$MMMMMMMMMMMMMM                        
    'MMMMMMMMMMMMMMMMMMMMMMMMMM           ");

            pictures.Add(@"        @RBS     ^KBRRS                                 
                          ^RRR#   ~RRRR@t                                 
                           BRBRBBBBRBBR@BSCt G@@@@#                       
                          (BRRRRRRRRRRRRB@@@@@B@RB@(                      
                CQR###GSs#BRRRRRRRRRRRRRRRBRBRBRK^                        
                KBRRRRRRRRBBBRRRBBRBBBBRBBBBBRBG                          
                   %BRRBRRRRRBBBBRBBRBBBBBBBBBB@C                         
                   (#BRRRRRRRRRRBRB#BB#BBRBBRRRB@s                        
              ^RBRRBBRRRBRRRRBBGssss6s666K@BRBS@@S                        
             %BRRBRBBBRBRBRBB@RCsss6s66666RBRBCKK                         
             (^   BBBRRBRRRR@G6s6666666s66QBBGC66K%                       
                 %RRBRBRRRRRBCssssss6ssss66G#Csss6sO                      
              ~SBBRBRBRRBBRRRCssss6GQQQKKQs6sssQG((CS                     
              C@B#RBBRRRB@RRRCs66G^ ^^^^R^SRKKK^^tR ^C                    
                 (@BR@@SKS@@@%ss6G/ ^ ^ ^/KCC66GC  ^(                     
                  @GCCG6s6G@@C6s66GGGGGOOCsssss66O#K                      
                  QsQss6s666666666666s66666666s66666K                     
                   GQQ6666666666s6666666ssGQKQQQQG6OR                     
                    GGs6s6s66666666sssss6666666666666K                    
                  QB@#sssssssssssss6sssss666sssssssss6Q                   
              ~#R##@@@Cssssssss66666GQQGs6sGQQGsss666#C          ~QKK/    
             /R#R##@@@#Cssssss666666G6666s66s666G(/^          SKRG6sGB(   
        SB@@@@R##R#B@BBRCssss666666666666ss6666BBB@         (QGGs6QC6sS   
       S@B@@@@@@#RR#@@@@@Q6Gsss6ssss66ssssss6QBRRBB@       #sssss6ssGG    
       B@BB@@@@@R#RR@@@@@//#Gssssssssssss6s6#B@@K@B@(     QssssssssG(     
      /BR##R#@@@@KRRR@@@@@    (QG66ssss6QQC O#@@KB@@#    t#C6ssss6G   ^   
     CBRR###R#@@@B#R#R@@@@K%C          ~ (#  S@@KR@@@    R%6ss666666666G  
     BBR##RRR#@@@B##R#@@@BB   ~O((~~(%CS/    %@@K#@B@    BQQG6ss66s66G(   
    SRRR##RRRR#@@@####R@@B@R                 (@@KK@@K^ (@####RR#CssG      
   GBRRR#R#R###@@@#RR##R@@@BR                ~@@K#@@#BR#RRRR###RRR        
   @KRRR##R#R##@@RRRRR#RR@@@@#               ~@@K#@@KRB########R#R");

            pictures.Add(@"                                                             
                                 ..uuedWWWeou.                          
                            u@$$$$$$$$$$$$$$$$$$$$o.                    
                        .@$$$$$$$$$$$$$$$$$$$$$$$$$$$$$                 
                     .@$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$&                
                  ud$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$                
                 ($$$$$$$$$$$R""~~~~""#*$$$$$$$$$$$$$R$$$E                
                 9$$$$$$$$$$K~~~~~~~~~~~~~~~""""""~~~~~9$$>                
                 M$$$$$$$$$$E~~~~~~~~~~~~~~~~~~~~~~~~$E                 
                 M$$$$$$$$$$B~~~~~~~~~~~~~~~~~~~~~~~~M                  
                 M$$$$$$$$$$$~~~~~~~~~~~~~~~~~~~~~~~~f                  
                 9$$$$$$$$$$$~~~~~~~~~~~~~~~~~~~~~~~~>                  
                 ~$$$$$$$$$$R~~~~~~~~~~~~~~~~~~~~~~~~h                  
                 '$$$$$$$$$""~~~~~~~~~~~~~~X~~~~X~~~~~~!                 
                  $$$$$$$$$(~~~~Xs@""""""""```F~~~~R#    :                  
                  ?$$$$$$$$!~~~~~?:     x""~~~~~~u..+""!                  
                  'E~~""$$$$!~~~~~~~~""?""~~~~~~~~~!~~~~~!                 
                  '~(:~~#$$!~~~~~~~~~~~~~~~~~~~~~!~~~~X                 
                  X~~!?~~""~~~~~~~~~~~~~~~~~~~~~~~~!~~~!                 
                  `~~~!~~~~~~~~~~~~~~~~~~~~~~~~~~~X~~~!                 
                   X~~!~~~~~~~~~~~~~~~~~~~~~~~~~~~!~~~X                 
                    %:~~~~~~~~~~~~~~~~~~~~~~~~~~~~~!~~X                 
                      $$$~~~~~~~~~~~~~~~~~~~uX*nu:X~~(>                 
                      @$$~~~~~~~~~~~~~~~~~~~~~~~~~~~~!                  
                       ""*!~~~~~~~~~~~~~~~~~~~~~~~~~~~~L                 
                         !~~~~~~~~~~~~~~~~~~~~~~~~~~~(!                 
                         !~~~~~~~~~~~~~~~~~~~~~~~:X*`                   
                         !~~~~~~?#""""~~~~~~~~~~~~~X                      
                        zE~~~~~~~~~~~~~~~~~~~~~~!                       
                      dMM?~~~~~~~~~~~~~~~~~~~~~~f                       
                     MMM '!~~~~~~~~~~~~~~~~~~~~(N                       
                   udMMM  "":~~~~~~~~~~~~~~~~~~~9MMh                     
               .@RMMRMMM    ""X~~~~~~~~~~~~~~~(f 9MMM$s                  
            .mRMMMM$MMMM>     '""i~~~~~~~~~:z""   'MMMM$MRu               
          dRMMMMMMMRMMMMk            MTT?!!!R    9MMMM$MMMM.            
       zRMMMMMMMMM$MMMMME%         /`X!!!!!!M :  9MMMMM$MMMMR.          
    uRMMMMMMMMMMMMMMMMMMM 4       f  4!!!!!!f '> 9MMMMMM$MMMMMR.        
 :MMMMMMMMMMMMMMM$MMMMMMM> `     f    M!!!!@   ?: RMMMMMM$MMMMMMN       
RMMMMMMMMMMMMMMMMMMMMMMMME   \  !      RUU@R    F 9MMMMMMM@R$MMMMMc     
MRMMMMMMMMMMMM$8$MMMMMMMMM>   `s~      X!!!M      `MMMMM$RMMMMMMMMMR.   
MMMBMMMMMMMMMMMMMR8MMMMMMMM           X!!!!X      'MMMMRMMMMMMMMMMMMMk  
MMMMMMMMMMMMMM$MMMMMR8MMMMMM          X!!!!!>      9MRMMMMMMMMM$MMMMMMN 
MMMM$MMMMMMMMMMBMMMMMMMMMMMMR         X!!!!!t      ?MMMM@@MMMMMRMMMMMMMR");

            pictures.Add(@"

                                                  .z@c      
                                             .u@RMMM$$$L    
                                         u@RMMMMMMM$$$$$$.  
                                        tMMMMMMMMMM$$$$$$$N.
                                       'RMMMMMMMMM$$$$$$$$$$
                                       @MMMMMMMMM$$$$$$$$$$$
                u@MT??T?s.            dMMMMMMMMMM$$$$$$$$$$f
              M!!!!!!!!!!!R.          RMMMMMM88@$$$$$$$$$$E 
             X!!!!!!!!!!!!!!L        @MMMMMMRM88RMR$$$$$$$> 
            '!!!!!!!!!!!!!!!!>      XMMMMMM$MMM$MR8$$$$$$R  
            '!!!!!!!!!!!!!!!!X      RMMMMM$MMM$M$MM$$$$$$   
            'X!!!f""""*U!!!!!!!!M    9MMMMM$@RRMR$MM$$$$$$F   
             ?X!X.  o  ?!M""`. `f  :MMMMM$8@@R$$MMR$$$$$$    
              X!!k     X!X    '>  $MMMM$MM88$8MM$$$$$$$f    
              'X!!???TX!!!T%@MX  9MMMMMRMMMMRMR$M$$$$$R     
               X!!!!!!!!!!!!%XX (RMMMM$@RRM$M$MMM$$$$$>     
               X!!!!!!!!!!!!!!> $MMMMRM8@@$8MMM$$R!$$E      
               X!!!X!!H!!!!X!!k9MMMM$?!!MMRMM$8#?!X$$       
               X!!X!!!K!!M!!X!!MMMMR!!!MR$8M$T!!!!@$F       
              zX!!X!!MX!!M!!M!!RMMR!!!!MM$T!!!!!!!$$        
            /!~&!!!!!!?@?!tH$MMMMM!!!!?!!!!!!!!!!@$f        
          :!~~~?!??@XUUUUUX@$MMMM$!!!!!!!!!!!!!!X$R         
         X~~~~~~?X!!!!!!!X!$MMM*~~""%X!!!!!!!!!!X$$>         
        !~~~~~~~~?X!TTTTTT$MM*~~~~~~~""t!!!!!!!W$$E?x        
      :!~u!+!XiuL:~%!!!!!98#~~~~~~~~~~~?UUU@R$$$$!~~""x      
     :~:~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~?MMMR$$$E~~~~~%     
     !~!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~X8MMM$$$!~~~~~~!    
     !~!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~dRMMB$$$E~~~~~~~!>   
     !~~>~~~~~~~~~~~~~~~~~~~~~~~~~~~~X$8@?!!!!!!!%i~~~~!    
     !~~~t~~~~~~~~~~~~~~~~~~~~~~~~(dRMMR!!!!!!!!!!!!(~~!    
     !~~~~~""%L~~~~~~~~~~~~~~~~~~(d$MR$$BRMM@?!!!!!!!%~(>    
     !~~~~""!%!""~?%XL:~~~~~~~(u@RMMRMMMMMMRT?!!!!!!!!X~X     
    :!~~~~~~~~~~~!~~XZRRRRRRRRRRR$$N8MMM*T!!!!!!!!!X~X      
   :!~~~~~~~~~~~~!~X$$$MMMM88$$$$$$8M@TT!!!!!!!!!!@+""       
   !~~~~~~~~~~~~!!~~$$$$$$$$$$$$$$$T!!!!!!!!!!!!Xf          
  X~:~~~~~~~~~~~X~~~!R***$$$$$$$$$$$$$WUUX!!!XX*            
 '~~!~?+u:~~~~~(!!~?~~~~~~$$$$$$$$$$$$R""~~~~!               
 !~!~~~~~~~~~""?!~~~~~~~~~~~~~~~~~~?R~~~~~~~~~>              
'!~!~~~~~~~~~!~~(~~~~%:~~~~~~~~~~~~~~~~~~~~~~!              
!~~!~~~~~~~~~!~~!~~~~~~?t~~~~~~~~~~~~~~~~~~~~!              
!~~!~~~~~~~~~!~~!~~~~~~~~~""%i:~~(X*""~~~~~~~~~~>             
!~~!~~~~~~~~(!~~!~~~~~~~~~~~~~~~~~~~~~~~~~~~~~>             
!~~~%:~~~~~~!~~~!~(~~~~~~~~~~~~~~~~~~~~~~~~~~~!             
 ""i~~~~""?%++""~~~!!:X~~~~~~~~~~~~~~~~~~~~~~~~~~! ");

            pictures.Add(@"
                       .   'u                              
                       `?x '~?c                            
                        X~?d~~~t                           
                :*""~""#%mU~~~~~~~M                          
           M%L #~~~~~~~~~~~~~~~~~k                         
         ...k~~~~~~~~~~~~~~~~~~~~X                         
         ""i~~~~~(z++iL~~~~~~~~~~~X                         
           `:~~~~~c     """"%mmm+m""                          
           f~~~~T""t9           `                           
          '~~~~~~?   x""""^""x  u=**u                         
           X~~~~~~M X      t>   . %                        
           'L~~~~~! X  '   X'h    X                        
             ""tumi~> %.  .f    ""mM                         
               !R%          +u..M'L                        
               'L                 'L                       
                  X                 %                      
                  !    d$$$$$`F""````                       
                 u@   ?iuu9$* (                            
                M!M          X9                            
             .ud!!!>         M!t                           
        uM?!!!!X!!!?i      .u!!!N!Ts                       
      :R!!!!!!!9!!!!!t ``   '!!!!&!!!k                     
     @!!!!?K!!!!&!!W#X5      K@Ttt!!!!k                    
    @!!!!!!9!!!!?@?!!?9      R!!!!!!!!!k                   
    E!!!!!!!B!!!!!!!!!9      4!!!!!!M!!M                   
    E!!!!!XU$W@$$$$$$$$$$$$$$$$$$NWWNWUUk                  
   d$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$L                
  $$$$$$$$$$$$$$$$$$$$$$*$*$RR$$$$$$$$$$$$$L               
  $$$$*$TT?$!!!$!#!!!!9     '  9!!!!!$!!!!T5               
  M!!!!!!!!$!!!!!!!!!!X         N!!!!!&!!WWU!              
 '!!!!!!!!!$!!!!!!!!!!R          E!!!!!5!!!!?9uu           
 9W$$$$WX!!K!!!!!!!!!!K          E!!!!!$!!!W$$$$$N         
d$$$$$$$$$XE!!!!!!!!!!!L         E!!!!!t!!$$$$$*TTIh.      
#$TUW@UU?$$!!!!!!!!!!!!M         5!!!!!T&$$$$$!@!X*""""""x    
 'XU@@@UX#R!!!!!!!!!!!!!L.uueemmmmK!!!!!!9*$$!R!F       2L 
 ""       ""X!!!!!!!!!!!!!RMMMMMMMMMM!!!!!!!t ""W!M   zi 'k'>h
X    .     `X!!XW@T!!!!!$MMMMMMMMMMUUUUUUX!>   #  '  h X M'
X !  M  !'  N#!!!!!!UUUU$MMMMMMMMMMMMMMMMMML    'u J t.C.H""
 %': )  \ kd!!XU@RMMMMMMMMMMMMMMMMMMMMMMMMMMN              
    """"""=* `*$RMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMR.            
            MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMk           
             RMMMMMMMMMMMMMMMMM88$RMMMMMMMMMMMMMR          
              RMMMMMMMMMMMMMMMM! #MMMMMMMMMMMMMMMML");

            pictures.Add(@"    u                                 
      .  x!X                                 
    .""X M~~>                                 
   d~~XX~~~k    .u.xZ `\ \ ""%                
  d~~~M!~~~?..+""~~~~~?:  ""    h              
 '~~~~~~~~~~~~~~~~~~~~~?      `              
 4~~~~~~~~~~~~~~~~~~~~~~>     '              
 ':~~~~~~~~~~(X+"""" X~~~~>    xHL             
  %~~~~~(X=""      'X""!~~% :RMMMRMRs          
   ^""*f`          ' (~~~~~MMMMMMMMMMMx       
     f     /`   %   !~~~~~MMMMMMMMMMMMMc     
     F    ?      '  !~~~~~!MMMMMMMMMMMMMM.   
    ' .  :"": ""   :  !X""""(~~?MMMMMMMMMMMMMMh  
    'x  .~  ^-+=""   ? ""f4!*  #MMMMMMMMMMMMMM.
     /""               ..""     `MMMMMMMMMMMMMM
     h ..             '         #MMMMMMMMMMMM
     f                '          @MMMMMMMMMMM
   :         .:=""""     >       dMMMMMMMMMMMMM
   ""+mm+=~(""           RR     @MMMMMMMMMMMMM""
           %          (MMNmHHMMMMMMMMMMMMMMF 
          uR5         @MMMMMMMMMMMMMMMMMMMF  
        dMRMM>       dMMMMMMMMMMMMMMMMMMMF   
       RM$MMMF=x..="" RMRM$MMMMMMMMMMMMMMF    
      MMMMMMM       'MMMMMMMMMMMMMMMMMMF     
     dMMRMMMK       'MMMMMMMMMMMMMMMMM""      
     RMMRMMME       3MMMMMMMMMMMMMMMM        
    @MMMMMMM>       9MMMMMMMMMMMMMMM~        
   'MMMMMMMM>       9MMMMMMMMMMMMMMF         
   tMMMMMMMM        9MMMMMMMMMMMMMM          
   MMMM$MMMM        9MMMMMMMMMMMMMM          
  'MMMMRMMMM        9MMMMMMMMMMMMM9          
  MMMMMMMMMM        9MMMMMMMMMMMMMM          
  RMMM$MMMMM        9MMMMMMMMMMMMMM          
 tMMMMMMMMMM        9MMMMMMMMMMMMMX          
 RMMMMMMMMMM        9MMMMMMMMMMMMME          
JMMMMMMMMMMM        MMMMMMMMMMMMMME          
9MMMM$MMMMMM        RMMMMMMMMMMMMME          
MMMMMRMMMMMX        RMMMMMMMMMMMMMR          
RMMMMRMMMMME        EMMMMMMMMMMMMM!          
9MMMMMMMMMME        MMMMMMMMMMMMMM>   ");
            pictures.Add(@"                                           _..--------.._              
   ________________________              _.-e        ||    e-._
 /'                    \ ||``\         /'    \\  |      |  |   `\
|:    ===               -||:  )      /'   \   _.--eeee--._     / `\
 \.____________________/_||../     /'  \    /e            e\ /  /  `\
    |       |                     ||    \ /'     \||  |/    `\       \
    |    || `\     ___-----------/e\  \  /    \           /   \  /    \
     \    \\  \.-ee   \  |  /   /|  | - |   \   .-eeee-.    /  \     - ;
     |           \  \     |  _/' |O |  |e\ __./'        `\   _  | - _  |
     |O    /____________..--e [] |__|- | [] ..   .---._   |  -  | _   _|
     |   ===____________ --|]=====__|  | [] ..  ( D>   )  |  _  |  _  =|
     |O    \  /    /    ee--. [] |  | -| []__    `---'e   |  _  |  -  e|
     |           /   /    |  `\  |O |  |_/   `\          /      | -    |
     /    //  .-.__   / |   \  `\|  | _ |   /  `-.____.-'   \  /    \  ;
    |    ||  /     eee------------\_/     \   /            \   /      /
   _|_______|______________       || /    \      /|  | |\    /  \    /
   _|_______|______________       || /    \      /|  | |\    /  \    /
 /'                    \ ||``\     \   / / `\_             /'  \    /
|:   ===                -||:  )     `\        e-__    __.-e \  \  /'
 \.____________________/_||../        `\  /   /   eeee |        /'            
                                        `-._     |  ||    \ _.-'              
                                            e-..________..-e  ");

            pictures.Add(@"                   ,.ood888888888888boo.,
               .od888P^""""            """"^Y888bo.
           .od8P''   ..oood88888888booo.    ``Y8bo.
        .odP'""  .ood8888888888888888888888boo.  ""`Ybo.
      .d8'   od8'd888888888f`8888't888888888b`8bo   `Yb.
     d8'  od8^   8888888888[  `'  ]8888888888   ^8bo  `8b
   .8P  d88'     8888888888P      Y8888888888     `88b  Y8.
  d8' .d8'       `Y88888888'      `88888888P'       `8b. `8b
 .8P .88P            """"""""            """"""""            Y88. Y8.
 88  888                                              888  88
 88  888                                              888  88
 88  888.        ..                        ..        .888  88
 `8b `88b,     d8888b.od8bo.      .od8bo.d8888b     ,d88' d8'
  Y8. `Y88.    8888888888888b    d8888888888888    .88P' .8P
   `8b  Y88b.  `88888888888888  88888888888888'  .d88P  d8'
     Y8.  ^Y88bod8888888888888..8888888888888bod88P^  .8P
      `Y8.   ^Y888888888888888LS888888888888888P^   .8P'
        `^Yb.,  `^^Y8888888888888888888888P^^'  ,.dP^'
           `^Y8b..   ``^^^Y88888888P^^^'    ..d8P^'
               `^Y888bo.,            ,.od888P^'
                    ""`^^Y888888888888P^^'""         LS");
            pictures.Add(@":::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:::::::::::::::::::::::::::::::::::::::::::::-'    `-::::::::::::::::::
::::::::::::::::::::::::::::::::::::::::::-'          `::::::::::::::::
:::::::::::::::::::::::::::::::::::::::-  '   /(_M_)\  `:::::::::::::::
:::::::::::::::::::::::::::::::::::-'        |       |  :::::::::::::::
::::::::::::::::::::::::::::::::-         .   \/~V~\/  ,:::::::::::::::
::::::::::::::::::::::::::::-'             .          ,::::::::::::::::
:::::::::::::::::::::::::-'                 `-.    .-::::::::::::::::::
:::::::::::::::::::::-'                  _,,-::::::::::::::::::::::::::
::::::::::::::::::-'                _,--:::::::::::::::::::::::::::::::
::::::::::::::-'               _.--::::::::::::::::::::::#####:::::::::
:::::::::::-'             _.--:::::::::::::::::::::::::::#####:::::####
::::::::'    ##     ###.-::::::###:::::::::::::::::::::::#####:::::####
::::-'       ###_.::######:::::###::::::::::::::#####:##########:::####
:'         .:###::########:::::###::::::::::::::#####:##########:::####
     ...--:::###::########:::::###:::::######:::#####:##########:::####
 _.--:::##:::###:#########:::::###:::::######:::#####:#################
'#########:::###:#########::#########::######:::#####:#################
:#########:::#############::#########::######:::#######################
##########:::########################::################################
##########:::##########################################################
##########:::##########################################################
#######################################################################
#######################################################################
################################################################# dp ##
#######################################################################");

            pictures.Add(@"                  ..oo$00ooo..                    ..ooo00$oo..
                .o$$$$$$$$$'                          '$$$$$$$$$o.
             .o$$$$$$$$$""             .   .              ""$$$$$$$$$o.
           .o$$$$$$$$$$~             /$   $\              ~$$$$$$$$$$o.
         .{$$$$$$$$$$$.              $\___/$               .$$$$$$$$$$$}.
        o$$$$$$$$$$$$8              .$$$$$$$.               8$$$$$$$$$$$$o
       $$$$$$$$$$$$$$$              $$$$$$$$$               $$$$$$$$$$$$$$$
      o$$$$$$$$$$$$$$$.             o$$$$$$$o              .$$$$$$$$$$$$$$$o
      $$$$$$$$$$$$$$$$$.           o{$$$$$$$}o            .$$$$$$$$$$$$$$$$$
     ^$$$$$$$$$$$$$$$$$$.         J$$$$$$$$$$$L          .$$$$$$$$$$$$$$$$$$^
     !$$$$$$$$$$$$$$$$$$$$oo..oo$$$$$$$$$$$$$$$$$oo..oo$$$$$$$$$$$$$$$$$$$$$!
     {$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$}
     6$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$?
     '$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$'
      o$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$o
       $$$$$$$$$$$$$$;'~`^Y$$$7^''o$$$$$$$$$$$o''^Y$$$7^`~';$$$$$$$$$$$$$$$
       '$$$$$$$$$$$'       `$'    `'$$$$$$$$$'     `$'       '$$$$$$$$$$$$'
        !$$$$$$$$$7         !       '$$$$$$$'       !         V$$$$$$$$$!
         ^o$$$$$$!                   '$$$$$'                   !$$$$$$o^
           ^$$$$$""                    $$$$$                    ""$$$$$^
             'o$$$`                   ^$$$'                   '$$$o'
               ~$$$.                   $$$.                  .$$$~
                 '$;.                  `$'                  .;$'
                    '.                  !                  .`
");
            pictures.Add(@"     ,-._.-._.-._.-._.-.
       `-.             ,-'
         |             | 
         |             | 
         |             | 
         |    .. ,.    |  
       ,-|___|  |  |___|-.
       | |   L__;__J   | |
       `|      / \      |'
        |     (   )     |
        |      `''      |
        |               |
        |               |
        ;--..._____...--;
       ,'--.._/   \_..--`.
      /       `. ,'       \
     /    /`.  | |    _l_  \
    /_/   \  \_J |  |""   |\_\
    //     `-.__.'  |    | \\
   ||           |   `---'   ||
   ||           |           || 
  ||            |            ||
  ||            |            ||
  ||            |            ||
 ;' |           |           | `; 
  `' \          |          / `'
      `--..____/ \____..--'
        |       |       |
        |       |       |
        |       |       |
        |       |       |
        |       |       |
        |       |       |
        |       |       |
        |       |       |
        |       |       |
        |       |       |
        |_______|_______|
  _..--'      |   |      `--.._   
 ;________.___|   |___.________;   
");
            pictures.Add(@"        ,,########################################,,
          .*##############################################*
        ,*####*:::*########***::::::::**######:::*###########,
      .*####:    *#####*.                 :*###,.#######*,####*.
     *####:    *#####*                      .###########*  ,####*
  .*####:    ,#######,                        ##########*    :####*
  *####.    :#########*,                       ,,,,,,,,.      ,####:
    ####*  ,##############****************:,,               .####*
     :####*#####################################**,        *####.
       *############################################*,   :####:
        .#############################################*,####*
          :#####:*****#####################################.
            *####:                  .,,,:*****###########,
             .*####,                            *######*
               .####* :*#######*               ,#####*
                 *###############*,,,,,,,,::**######,
                   *##############################:
                     *####*****##########**#####*
                      .####*.            :####*
                        :####*         .#####,
                          *####:      *####:
                           .*####,  *####*
                             :####*####*
                               *######,
                                 *##,
");
            pictures.Add(@"                                                . 
                                               /| 
                          ____...,---.___..__,) | 
    ,_.-._   ,._,.--''''''                   ' ; 
   /      '-'                               ) / 
 ,'_._.    `_                             ,' : 
       `-.   -.                    _     '  ; 
         )`-.  `.     /;          ) )   ,' / 
          '. `.  -.  (  \        | =' .'  ; 
            `. '-._+ .\= \.-'-'-.| ; /  :' 
              `.    \\ `-        `.+'  ; 
       .-.      `. ,-+,' = _  (_  ;  .' 
       ; (_      /: =,(    O\  O  ).J 
       `. =`.   ;'  =\ \     7P   ' 
         `-. '-: .' = \ \  (:  :) ) 
            `--| ;   = `.`. `==""./ 
               ; :       `-`.__.' ) 
               : |                ; 
               '.'.=  -.     .    + 
            ,'`)  :=    \    ,  /  `. 
            ) =|   `.   ,+-.-'-+.    ;. 
            ; \:   /|   | ;      `._  =) 
           :  ( `-' |   (/  kOs     ;  ; 
           ( |;    (:    )          |  : 
            `-     / )|  ;          )  |) ");
            pictures.Add(@"                _                         
                              _(_)                        
                          .-*""    `.                      
                          L_____   .""T  _                 
                          :\  `,`:.`""\.' ;                
                          |_, _: : `.,+._:                
                         /;s; s'  \  ;P;_.                
                        .+*'  `_   `.J-  :                
                       :   *'._,*,  (J)-.*                
                       :      /""""*+' (L)                  
                  s..s()l-._.'--*""    `()  _              
            ___   _T$$o ;             T$bs$P-._           
  .-*'""*+.*""   ""*""  d$b `          /   T$*Tb   `--*""""*.   
 /        \        sP Ts `-.____.-'    dP  `*          `. 
(`         ,      /        .'                     _    `.\
 \         :    _:       .'                     .'       `)
  \         L_.' ;___   /                      /         / 
   `-.    .' ._.'_   """"**--..__         _    //         /  
      `""*/    /   \.*""""*.  .-._""""**--._: \.-'/         /   
        (((( :"",  / .-. ;'  _ """"**--.._\ `-+        ,'     
        :.;: :""  :  *-' /  : ;          `:   `   _.-'      
         \:; ;._.'`._.-'`,  ""           .',    :')         
         /;: ;     ;      `""""**--..___ / /; ,,,::          
        : `'T                   /    \T /-;.',.':          
            ;     :            :      \""   ""*T""*(          
       :          ;            ;       ;      \  ;         
       :   :      :                    :       ; :         
       ;   ;                  :                : :         
       :   :       ;          ;         ;      : :         
                   :          :         :      ; ;         
        \   \       \                   ;                  
         `.  `.      \         ;              : /          
           `.  `.     `.       :     .'/.    /.'           
             `*-.`-.__  `-.     \   /.'\  ,.+'             
                 ""*-._:-'  J__..-+-*""\  ; :                
                      ; -*'/          ; : ;                
                      :   :   [bug]   : ;                  
                       \   `.         : |:                 
                      .-`.__J.      .-; |;._ _.-**-.       
                   .--`-.___.-'-.  :.'`""'\_.' __..-*;      
               .*""( ___         : : `""""""*-'.-'__...-'      
               ; .'`-._""*-._.--*' `""*----**""""'             
               `***---+***'                           ");
            pictures.Add(@"
                                      .M
                                 .:AMMO:
                        .:AMMMMMHIIIHMMM.
             ....   .AMMMMMMMMMMMHHHMHHMMMML:AMF""
             .:MMMMMLAMMMMMMMHMMMMMMHHIHHIIIHMMMML.
                 ""WMMMMMMMMMMMMMMMMMMH:::::HMMMMMMHII:.
            .AMMMMMMMHHHMMMMMMMMMMHHHHHMMMMMMMMMAMMMHHHHL.
          .MMMMMMMMMMHHMMMMMMMMHHHHMMMMMMMMMMMMMHTWMHHHHHML
         .MMMMMMMMMMMMMMMMMMMHHHHHHHHHMHMMHHHHIII:::HMHHHHMM.
         .MMMMMMMMMMMMMMMMMMMMMMHHHHHHMHHHHHHIIIIIIIIHMHHHHHM.
         MMMMMMMMMMMMMMMMMHHMMHHHHHIIIHHH::IIHHII:::::IHHHHHHHL
         ""MMMMMMMMMMMMMMMMHIIIHMMMMHHIIHHLI::IIHHHHIIIHHHHHHHHML
         .MMMMMMMMMMMMMM""WMMMHHHMMMMMMMMMMMLHHHMMMMMMHHHHHHHHHHH
        .MMMMMMMMMMMWWMW""""YYHMMMMMMMMMMMMF""""HMMMMMMMMMHHHHHHHH.
        .MMMMMMMMMM W"" V                         W""WMMMMMHHHHHHHHHH
       ""MMMMMMMMMM"".                                 ""WHHHMH""HHHHHHL
       MMMMMMMMMMF  .                                         IHHHHH.
       MMMMMMMMMM .                                  .        HHHHHHH
       MMMMMMMMMF. .                               .  .       HHHHHHH.
       MMMMMMMMM .     ,AWMMMMML.              ..    .  .     HHHHHHH.
     :MMMMMMMMM"".  .  F""'    'WM:.         ,::HMMA, .  .      HHHHMMM
     :MMMMMMMMF.  . .""         WH..      AMM""'     ""  .  .    HHHMMMM
      MMMMMMMM . .     ,;AAAHHWL""..     .:'                   HHHHHHH
      MMMMMMM:. . .   -MK""OTO L :I..    ...:HMA-.             ""HHHHHH
 ,:IIIILTMMMMI::.      L,,,,.  ::I..    .. K""OTO""ML           'HHHHHH
 LHT::LIIIIMMI::. .      '""""'.IHH:..    .. :.,,,,           '  HMMMH: HLI'
 ILTT::""IIITMII::.  .         .IIII.     . '""""""""             ' MMMFT:::.
 HML:::WMIINMHI:::.. .          .:I.     .   . .  .        '  .M""'.....I.
 ""HWHINWI:.'.HHII::..          .HHI     .II.    .  .      . . :M.',, ..I:
  ""MLI""ML': :HHII::...        MMHHL     :::::  . :..      .'.'.'HHTML.II:
   ""MMLIHHWL:IHHII::....:I:"" :MHHWHI:...:W,,""  '':::.      ..'  "":.HH:II:
     ""MMMHITIIHHH:::::IWF""    """"""T99""'  '""""    '.':II:..'.'..'  I'.HHIHI'
       YMMHII:IHHHH:::IT..     . .   ...  . .    ''THHI::.'.' .;H."""".""H""
         HHII:MHHI""::IWWL     . .     .    .  .     HH""HHHIIHHH"":HWWM""
          """""" MMHI::HY""""ML,          ...     . ..  :""  :HIIIIIILTMH""
               MMHI:.'    'HL,,,,,,,,..,,,......,:"" . ''::HH ""HWW
               'MMH:..   . 'MMML,: """"""MM""""""""MMM""      .'.IH'""MH""
                ""MMHL..   .. ""MMMMMML,MM,HMMMF    .   .IHM""
                  ""MMHHL    .. ""MMMMMMMMMMMM""  . .  '.IHF'
                    'MMMML    .. ""MMMMMMMM""  .     .'HMF
                     HHHMML.                    .'MMF""
                    IHHHHHMML.               .'HMF""
                    HHHHHHITMML.           .'IF..
                    ""HHHHHHIITML,.       ..:F...
                     'HHHHHHHHHMMWWWWWW::""......
                       HHHHHHHMMMMMMF""'........
                        HHHHHHHHHH............
                          HHHHHHHH...........
                           HHHHIII..........
                            HHIII..........
                             HII.........
                              ""H........
                                ...... ");
            pictures.Add(@"                        <xeee..                             
                                      ueeeeeu..^""*$e.                         
                               ur d$$$$$$$$$$$$$$Nu ""*Nu                      
                             d$$$ ""$$$$$$$$$$$$$$$$$$e.""$c                    
                         u$$c   """"   ^""^**$$$$$$$$$$$$$b.^R:                  
                       z$$#""""""           `!?$$$$$$$$$$$$$N.^                  
                     .$P                   ~!R$$$$$$$$$$$$$b                  
                    x$F                 **$b. '""R).$$$$$$$$$$                 
                   J^""                           #$$$$$$$$$$$$.               
                  z$e                      ..      ""**$$$$$$$$$               
                 :$P           .        .$$$$$b.    ..  ""  #$$$$              
                 $$            L          ^*$$$$b   ""      4$$$$L             
                4$$            ^u    .e$$$$e.""*$$$N.       @$$$$$             
                $$E            d$$$$$$$$$$$$$$L ""$$$$$  mu $$$$$$F            
                $$&            $$$$$$$$$$$$$$$$N   ""#* * ?$$$$$$$N            
                $$F            '$$$$$$$$$$$$$$$$$bec...z$ $$$$$$$$            
               '$$F             `$$$$$$$$$$$$$$$$$$$$$$$$ '$$$$E""$            
                $$                  ^""""""""""""`       ^""*$$$& 9$$$$N             
                k  u$                                  ""$$. ""$$P r            
                4$$$$L                                   ""$. eeeR             
                 $$$$$k                                   '$e. .@             
                 3$$$$$b                                   '$$$$              
                  $$$$$$                                    3$$""              
                   $$$$$  dc                                4$F               
                    RF** <$$                                J""                
                     #bue$$$LJ$$$Nc.                        ""                 
                      ^$$$$$$$$$$$$$r                                         
                        `""*$$$$$$$$$                                          
                $. .$ $~$ $~$ ~$~  $  $    $ $ $~$ $. .$ $~$  $  ~$~          
                $$ $$ $ $ $ $  $  $.$ $    $$  $ $ $$ $$ $.$ $.$  $           
                $`$'$ $ $ $~k  $  $~$ $    $$  $ $ $`$'$ $ $ $~$  $           
                $ $ $ $o$ $ $  $  $ $ $oo  $ $ $o$ $ $ $ $o$ $ $  $     ");
            pictures.Add(@"                                                ___,------, 
             _,--.---.                         __,--'         / 
           ,' _,'_`._ \                    _,-'           ___,| 
          ;--'       `^-.                ,'        __,---'   || 
        ,'               \             ,'      _,-'          || 
       /                  \         _,'     ,-'              || 
      :                    .      ,'     _,'                 |: 
      |                    :     `.    ,'                    |: 
      |           _,-      |       `-,'                      :: 
     ,'____ ,  ,-'  `.   , |,         `.                     : \ 
     ,'    `-,'       ) / \/ \          \                     : : 
     |      _\   o _,-'    '-.           `.                    \ \ 
      `o_,-'  `-,-' ____   ,` )-.______,'  `.                   : : 
       \-\    _,---'    `-. -'.\  `.  /     `.                  \  \ 
        / `--'             `.   \   \:        \                  \,.\ 
       (              ____,  \  |    \\        \                 :\ \\ 
        )         _,-'    `   | |    | \        \                 \\_\\ 
       /      _,-'            | |   ,'-`._      _\                 \,' 
       `-----' |`-.           ;/   (__ ,' `-. _;-'`\           _,--' 
     ,'        |   `._     _,' \-._/  Y    ,-'      \      _,-' 
    /        _ |      `---'    :,-|   |    `     _,-'\_,--'   \ 
   :          `|       \`-._   /  |   '     `.,-' `._`         \ 
   |           _\_    _,\/ _,-'|                     `-._       \ 
   :   ,-         `.-'_,--'    \                         `       \ 
   | ,'           ,--'      _,--\           _,                    : 
    )         .    \___,---'   ) `-.____,--'                      | 
   _\    .     `    ||        :            \                      ; 
 ,'  \    `.    )--' ;        |             `-.                  / 
|     \     ;--^._,-'         |                `-._            _/_\ 
\    ,'`---'                  |                    `--._____,-'_'  \ 
 \_,'                         `._                          _,-'     ` 
          -hrr-             ,-'  `---.___           __,---' 
                          ,'             `---------' 
                        ,' ");

            pictures.Add(@"
          _ _,---._ 
       ,-','       `-.___ 
      /-;'               `._ 
     /\/          ._   _,'o \ 
    ( /\       _,--'\,','""`. ) 
     |\      ,'o     \'    //\ 
     |      \        /   ,--'""""`-. 
     :       \_    _/ ,-'         `-._ 
      \        `--'  /                ) 
       `.  \`._    ,'     ________,',' 
         .--`     ,'  ,--` __\___,;' 
          \`.,-- ,' ,`_)--'  /`.,' 
           \( ;  | | )      (`-/ 
             `--'| |)       |-/ 
               | | |        | | 
               | | |,.,-.   | |_ 
               | `./ /   )---`  ) 
              _|  /    ,',   ,-' 
     -hrr-   ,'|_(    /-<._,' |--, 
             |    `--'---.     \/ \ 
             |          / \    /\  \ 
           ,-^---._     |  \  /  \  \ 
        ,-'        \----'   \/    \--`. 
       /            \              \   \ 
");

            pictures.Add(@"

                              . .  ,  , 
                              |` \/ \/ \,', 
                              ;          ` \/\,. 
                             :               ` \,/ 
                             |                  / 
                             ;                 : 
                            :                  ; 
                            |      ,---.      / 
                           :     ,'     `,-._ \ 
                           ;    (   o    \   `' 
                         _:      .      ,'  o ; 
                        /,.`      `.__,'`-.__, 
                        \_  _               \ 
                       ,'  / `,          `.,' 
                 ___,'`-._ \_/ `,._        ; 
              __;_,'      `-.`-'./ `--.____) 
           ,-'           _,--\^-' 
         ,:_____      ,-'     \ 
        (,'     `--.  \;-._    ; 
        :    Y      `-/    `,  : 
        :    :       :     /_;' 
        :    :       |    : 
         \    \      :    : 
          `-._ `-.__, \    `. 
             \   \  `. \     `. 
           ,-;    \---)_\ ,','/ 
           \_ `---'--'"" ,'^-;' 
           (_`     ---'"" ,-') 
           / `--.__,. ,-'    \ 
  -hrr-    )-.__,-- ||___,--' `-. 
          /._______,|__________,'\ 
          `--.____,'|_________,-' ");

            pictures.Add(@"                   _ 
                          ,' \ 
                        ,'    \...-. 
                      ,'           | 
                    ,'             | 
                 _,'---    -.      `-._ 
               ,'            `"".       ) 
            ,-'                 \     / 
        ,..'-                    `.  (           _ 
       (     ,--._     ,--._      `\  \        ,' | 
      ,-'   _...._\ _,.._   \      `\  )      /   \__ 
     /    ,'      ;'     `.          \/     ,/       ) 
    |    |       |         \          `.   ,'.     ,' 
    | O  :    O  |      O  |           '""--'  \   ( 
    `._  |       \         '                  |    \ 
     . --`.      _`-.___,,'                  ,'     ) 
   .-      `'--.'  `-.__,,-'                ,'   ,-' 
 ,'        '--.-'    .                  _       ( 
(                   _.`--.-        `-..' `.      `. 
 \             _,--'                  )   |   ,.--' 
  '-.......---' 'v'"""""".      _Y      /    `.  | 
     -hrr-      (_     ;----' |     (      `.,' 
                  \   /_)     '-.   _) 
          ___ _ _  `-' _        `..' 
         | _ ) (_)_ _ | |___  _ 
         | _ \ | | ' \| / / || | 
         |___/_|_|_||_|_\_\\_, | 
                           |__/ ");

            pictures.Add(@"         _          __________                              _,
     _.-(_)._     .""          "".      .--""""--.          _.-{__}-._
   .'________'.   | .--------. |    .'        '.      .:-'`____`'-:.
  [____________] /` |________| `\  /   .'``'.   \    /_.-""`_  _`""-._\
  /  / .\/. \  \|  / / .\/. \ \  ||  .'/.\/.\'.  |  /`   / .\/. \   `\
  |  \__/\__/  |\_/  \__/\__/  \_/|  : |_/\_| ;  |  |    \__/\__/    |
  \            /  \            /   \ '.\    /.' / .-\                >/-.
  /'._  --  _.'\  /'._  --  _.'\   /'. `'--'` .'\/   '._-.__--__.-_.'
\/_   `""""""""`   _\/_   `""""""""`   _\ /_  `-./\.-'  _\'.    `""""""""""""""""`'`\
(__/    '|    \ _)_|           |_)_/            \__)|        '        
  |_____'|_____|   \__________/|;                  `_________'________`;-'
  s'----------'    '----------'   '--------------'`--------------------`
     S T A N          K Y L E        K E N N Y         C A R T M A N");
            pictures.Add(@"                                                           _
                 _                     __  __          __     __ | |
       _______  | |  __  __           |  \/  |         \ \   / / | |
      |__   __| | | |  \/  |  ______  | \  / |     /\   \ \_/ /  | |
         | |    | | | \  / | |______| | |\/| |    /  \   \   /   |_|
         | |    | | | |\/| |          | |  | |   / /\ \   | |    (_)
         | |    |_| | |  | |          |_|  |_|  / ____ \  |_|
         |_|        |_|  |_|                   /_/    \_\
                            _______,.........._
                       _.::::::::::::::::::::::::._
                    _J::::::::::::::::::::::::::::::-.
                 _,J::::;::::::!:::::::::::!:::::::::::-.""\_ ___
              ,-:::::/::::::::::::/''''''-:/   \::::::::::::::::L_
            ,;;;;;::!::/         V               -::::::::::::::::7
          ,J:::::::/ \/                              '-'`\:::::::.7
          |:::::::'                                       \::!:::/
         J::::::/                                          `.!:\ dp
         |:::::7                                             |/\:\
        J::::/                                               \/ \:|
        |:::/                                                    \:\
        |::/                                                     |:.Y
        |::7                                                      |:|
        |:/                              `OOO8ooo._               |:|
        |/               ,,oooo8OO'           `""`Y8o,             |'
         |            ,odP""'                      `8P            /
         |          ,8P'    _.__         .---.._                /
         |           '   .-'    `-.    .'       `-.            /
         `.            ,'          `. /            `.          L_
       .-.J.          /              Y               \        /_ \
      |    \         /               |                Y      // `|
       \ '\ \       Y          8B    |   8B           |     /Y   '
        \  \ \      |                |                ;    / |  /
         \  \ \     |               ,'.              /    /  L |
          \  J \     \           _.'   `-._       _.'    /  _.'
           `.___,\    `._     _,'          '-...-'     /'`""'
                  \      '---'  _____________         /
                   `.           \|T T T T T|/       ,'
                     `.           \_|_|_|_/       .'
                       `.         `._.-..'      .'
                         `._         `-'     _,'
                            `--._________.--'");

            pictures.Add(@",~~~~~~~~~~~.
                       '  HIIII Dee  `
        ,~~~~~~,       |    Ho!      | 
       / ,      \      ',  ________,""
      /,~|_______\.      \/
     /~ (__________)   
    (*)  ; (^)(^)':
        =;  ____  ;
          ; """"""""  ;=
   {""}_   ' '""""' ' _{""}
   \__/     >  <   \__/
      \    ,""   "",  /
       \  ""       /""
          ""      ""=
           >     <
          =""     ""-
          -`.   ,'
                -
      SHW    `--'");

            pictures.Add(@"    .--..--..--..--..--..--.
    .' \  (`._   (_)     _   \
  .'    |  '._)         (_)  |
  \ _.')\      .----..---.   /
  |(_.'  |    /    .-\-.  \  |
  \     0|    |   ( O| O) | o|
   |  _  |  .--.____.'._.-.  |
   \ (_) | o         -` .-`  |
    |    \   |`-._ _ _ _ _\ /
    \    |   |  `. |_||_|   |
    | o  |    \_      \     |     -.   .-.
    |.-.  \     `--..-'   O |     `.`-' .'
  _.'  .' |     `-.-'      /-.__   ' .-'
.' `-.` '.|='=.='=.='=.='=|._/_ `-'.'
`-._  `.  |________/\_____|    `-.'
   .'   ).| '=' '='\/ '=' |
   `._.`  '---------------'
           //___\   //___\
             ||       ||
    LGB      ||_.-.   ||_.-.
            (_.--__) (_.--__)
");

            pictures.Add(@"    It's the classic story of two boys...sharing one brain....
 
             ,())))),
           ,()))))))),.       >>huh-huh<<         ,---,,,_
           ()))))))//((\     check it out,       (         ))   Plug into
          (\\( \))( \(/)    Beavis...we're,     (            )
          /(          \\  like, in ""ASS-kee.""   (            )  M T V
          //       _   \    >>huh-huh-huh<<     (_(_((((     )
          //   \  /    \   /                     (    , \    )  Music
          \   (.  .    \  /                      |   /   )   )
          (,     |    ,)     yeah. >>heh-heh<<   |\ /    (   )  Television
           \   ^\/^   /      that's COOL! hey,   (.(.)    S  )
           \          /     Butt-Head...you're    /_       \ )
            \ (-<>-) /         an ""ASS-kee.""  \  /__)   ^   \/
             \  --  /           >>heh-heh<<       /____/    |
              \ __ /                             )______    |
               |  |       //\/\\/\//\/\//\/\\\/\        \   |
            __-|__|-__   \                      /     __-\__|-__
           (          )  > BEAVIS AND BUTT-HEAD <    (          )
           |_|AC//DC|_|  /                      \    |_|METALL|_|
           | |      | |   \/\//\/\\/\\/\//\//\/\ TM  | |      | |  tif'94
 TM MTV Networks, a division of Viacom Int'l Inc. (mtvonline@aol.com).
Billboard artwork and design by The Image Factory (imagefact1@aol.com).");

           
            var num = UniqueRandom(0, pictures.Count-1).FirstOrDefault();
            return pictures[num];

        }

        static IEnumerable<int>  UniqueRandom(int minInclusive, int maxInclusive)
        {
            List<int> candidates = new List<int>();
            for (int i = minInclusive; i <= maxInclusive; i++)
            {
                candidates.Add(i);
            }
            Random rnd = new Random();
            while (candidates.Count > 0)
            {
                int index = rnd.Next(candidates.Count);
                yield return candidates[index];
                candidates.RemoveAt(index);
            }
        }
    }
}
