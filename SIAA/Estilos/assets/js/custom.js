jQuery(
    (function ($) {
        "use strict";
        $(window).on("scroll", function () {
            if ($(this).scrollTop() > 50) {
                $(".main-nav").addClass("menu-shrink");
            } else {
                $(".main-nav").removeClass("menu-shrink");
            }
        });
        jQuery(".mean-menu").meanmenu({ meanScreenWidth: "991" });
        $("#close-btn").on("click", function () {
            $("#search-overlay").fadeOut();
            $("#search-btn").show();
        });
        $("#search-btn").on("click", function () {
            $("#search-overlay").fadeIn();
        });
        $(function () {
            $(".common-btn")
                .on("mouseenter", function (e) {
                    var parentOffset = $(this).offset(),
                        relX = e.pageX - parentOffset.left,
                        relY = e.pageY - parentOffset.top;
                    $(this).find("span").css({ top: relY, left: relX });
                })
                .on("mouseout", function (e) {
                    var parentOffset = $(this).offset(),
                        relX = e.pageX - parentOffset.left,
                        relY = e.pageY - parentOffset.top;
                    $(this).find("span").css({ top: relY, left: relX });
                });
        });
        new WOW().init();
        $(".banner-slider").owlCarousel({ items: 1, loop: true, margin: 0, nav: false, dots: true, smartSpeed: 1000, autoplay: true, autoplayTimeout: 4000, autoplayHoverPause: true, animateOut: "fadeOut", animateIn: "fadeIn" });
        $(".banner-slider-two").owlCarousel({
            items: 1,
            loop: true,
            margin: 0,
            nav: true,
            dots: false,
            smartSpeed: 1000,
            autoplay: true,
            autoplayTimeout: 4000,
            autoplayHoverPause: true,
            animateOut: "fadeOut",
            animateIn: "fadeIn",
            navText: ["<i class='bx bx-chevron-left'></i>", "<i class='bx bx-chevron-right'></i>"],
        });
        $(".logo-slider").owlCarousel({
            loop: true,
            margin: 0,
            nav: false,
            dots: false,
            smartSpeed: 1000,
            autoplay: true,
            autoplayTimeout: 3000,
            autoplayHoverPause: true,
            responsive: { 0: { items: 2 }, 600: { items: 3 }, 1000: { items: 5 } },
        });
        $(".projects-slider").owlCarousel({
            loop: true,
            margin: 20,
            nav: true,
            dots: false,
            smartSpeed: 1000,
            autoplay: true,
            autoplayTimeout: 3000,
            autoplayHoverPause: true,
            navText: ["<i class='bx bx-chevron-left'></i>", "<i class='bx bx-chevron-right'></i>"],
            responsive: { 0: { items: 1 }, 600: { items: 2 }, 1000: { items: 3 } },
        });
        $(".js-modal-btn").modalVideo();
        $(".odometer").appear(function (e) {
            var odo = $(".odometer");
            odo.each(function () {
                var countNumber = $(this).attr("data-count");
                $(this).html(countNumber);
            });
        });
        $(".testimonials-slider").owlCarousel({
            items: 1,
            loop: true,
            margin: 0,
            nav: true,
            dots: false,
            smartSpeed: 1000,
            autoplay: true,
            autoplayTimeout: 3000,
            autoplayHoverPause: true,
            navText: ["<i class='bx bx-chevron-left'></i>", "<i class='bx bx-chevron-right'></i>"],
        });
        let getDaysId = document.getElementById("days");
        if (getDaysId !== null) {
            const second = 1000;
            const minute = second * 60;
            const hour = minute * 60;
            const day = hour * 24;
            let countDown = new Date("December 25, 2021 00:00:00").getTime();
            setInterval(function () {
                let now = new Date().getTime();
                let distance = countDown - now;
                (document.getElementById("days").innerText = Math.floor(distance / day)),
                    (document.getElementById("hours").innerText = Math.floor((distance % day) / hour)),
                    (document.getElementById("minutes").innerText = Math.floor((distance % hour) / minute)),
                    (document.getElementById("seconds").innerText = Math.floor((distance % minute) / second));
            }, second);
        }
        $(function () {
            $(window).on("scroll", function () {
                var scrolled = $(window).scrollTop();
                if (scrolled > 100) $(".go-top").addClass("active");
                if (scrolled < 100) $(".go-top").removeClass("active");
            });
            $(".go-top").on("click", function () {
                $("html, body").animate({ scrollTop: "0" }, 500);
            });
        });
        $(".accordion > li:eq(0) a").addClass("active").next().slideDown();
        $(".accordion a").on("click", function (j) {
            var dropDown = $(this).closest("li").find("p");
            $(this).closest(".accordion").find("p").not(dropDown).slideUp();
            if ($(this).hasClass("active")) {
                $(this).removeClass("active");
            } else {
                $(this).closest(".accordion").find("a.active").removeClass("active");
                $(this).addClass("active");
            }
            dropDown.stop(false, true).slideToggle();
            j.preventDefault();
        });
        jQuery(window).on("load", function () {
            jQuery(".loader").fadeOut(500);
        });
        $(".newsletter-form")
            .validator()
            .on("submit", function (event) {
                if (event.isDefaultPrevented()) {
                    formErrorSub();
                    submitMSGSub(false, "Please enter your email correctly.");
                } else {
                    event.preventDefault();
                }
            });
        function callbackFunction(resp) {
            if (resp.result === "success") {
                formSuccessSub();
            } else {
                formErrorSub();
            }
        }
        function formSuccessSub() {
            $(".newsletter-form")[0].reset();
            submitMSGSub(true, "Thank you for subscribing!");
            setTimeout(function () {
                $("#validator-newsletter").addClass("hide");
            }, 4000);
        }
        function formErrorSub() {
            $(".newsletter-form").addClass("animated shake");
            setTimeout(function () {
                $(".newsletter-form").removeClass("animated shake");
            }, 1000);
        }
        function submitMSGSub(valid, msg) {
            if (valid) {
                var msgClasses = "validation-success";
            } else {
                var msgClasses = "validation-danger";
            }
            $("#validator-newsletter").removeClass().addClass(msgClasses).text(msg);
        }
    })(jQuery)
);

