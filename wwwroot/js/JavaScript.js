
//LEI REF: http://stackoverflow.com/questions/20947529/what-does-ahref-nothref-code-mean

$(document).ready(function () {
    //LEI smooth scrolling for scroll to top 
    $('#to-top').bind('click', function () {
        $('body,html').animate({
            scrollTop: 0
        },
            2500);
    });

    // LEI Easing Scroll replace Anchor name in URL and Offset Position
    $(function () {
        $('a[href*=#]:not([href=#])').click(function () {
            if (location.pathname.replace(/^\//, '') === this.pathname.replace(/^\//, '') && location.hostname === this.hostname) {

                var target = $(this.hash);
                target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
                if (target.length) {
                    $('html,body').animate({
                        scrollTop: target.offset().top - 420
                    }, 3500, 'easeOutBounce');
                    return false;
                }
            }
        });
    });
});
$(document).ready(function () {
    $(".dropdown").hover(
        function () {
            $('.dropdown-menu', this).not('.in .dropdown-menu').stop(true, true).slideDown("400");
            $(this).toggleClass('open');
        },
        function () {
            $('.dropdown-menu', this).not('.in .dropdown-menu').stop(true, true).slideUp("400");
            $(this).toggleClass('open');
        }
    );
});



$(document).ready(function () {

    // LEI Closes the sidebar menu on menu-close button click event
    $("#menu-close").click(function (e)						
    {
   
        $("#sidebar-wrapper").toggleClass("active");			
        e.preventDefault(); 									

	
    });														

    // LEI Open the Sidebar-wrapper on Hover
    $("#menu-toggle").hover(function (e)							
    {
        $("#sidebar-wrapper").toggleClass("active", true);		
        e.preventDefault();										
    });

    $("#menu-toggle").bind('click', function (e)					
    {
        $("#sidebar-wrapper").toggleClass("active", true);		
        e.preventDefault();										
    });															

    $('#sidebar-wrapper').mouseleave(function (e)				
 
    {
       
        $('#sidebar-wrapper').toggleClass('active', false);		
													
																
        e.stopPropagation();						
     

        e.preventDefault();										
      
    });
});

$("#menu-close").click(function (e)							
{

    $("#sidebar-wrapper").toggleClass("active");			
    e.preventDefault(); 									

	
});															



	

