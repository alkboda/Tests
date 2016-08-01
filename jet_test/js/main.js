$(document).ready(function(){
	$('body').click(function() {
		var $ul = $('#mobile-main-menu ul');
		$ul.animate({
			opacity: 0
		}, 400, "linear", function() {
			$ul.hide();
		});
	});
	$('#mobile-main-menu').click(function(e) {
		var $ul = $('ul', this); console.log($ul.css('opacity'));
		if ($ul.css('opacity') == 0){
			$ul.show();
			$ul.animate({
				opacity: 1
			}, 400, "linear");
		}
		else {		
			$ul.animate({
				opacity: 0
			}, 400, "linear", function() {
				$ul.hide();
			});
		}
		e.stopPropagation();
	});
	
	$('.tabs a').click(function(e){
		if ($(this).parent().hasClass('current')) {
			return false;
		}
		
		$('.tabs .current').removeClass('current');
		$(this).parent().addClass('current');
		
		var target = $(this).data('target');
		var $old = $('#main-content .current');
		var $target = $('#main-content ' + target);
		
		$old.removeClass('current');
		$target.addClass('current');
		
		/* in case of animate on callbacks at fast cross-clicks 2 blocks will be visible */
		/* buttons could be disabled with enabling on callback but it's too late :( */
		$old.animate({
			opacity: 0
		}, 400, "linear");
		$target.animate({
			opacity: 1
		}, 700, "linear");
		
		e.stopPropagation();
		return false;
	});
});