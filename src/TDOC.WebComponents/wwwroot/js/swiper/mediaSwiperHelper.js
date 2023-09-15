function loadMediaData(s, swiperId, i) {
    mediaSwiperHelper.dotNetObjRefs[swiperId].invokeMethodAsync("GetMediaDataAsync", i)
        .then(() => { });

    s.slides[i].isLoaded = true;
}

function loadSlides(s, swiperId, forceLoad) {
    // No need to load default svg image.
    var notLoadedThumbs = s.visibleSlides.filter(x => !x.isLoaded || forceLoad);

    for (var i = 0; i < notLoadedThumbs.length; i++) {
        var index = s.slides.indexOf(notLoadedThumbs[i]);
        loadMediaData(s, swiperId, index);
    }
}

window.mediaSwiperHelper = {
    dotNetObjRefs: {},

    initSwiper: (dotNetReference, swiperId, hasThumbsSlider) => {
        mediaSwiperHelper.dotNetObjRefs[swiperId] = dotNetReference;
        if (hasThumbsSlider) {
            var swiperThumbs = new Swiper('.swiper-thumbnails-' + swiperId, {
                spaceBetween: 12,
                slidesPerView: 'auto',
                watchSlidesProgress: true,
                centerInsufficientSlides: true,
                centeredSlides: true,
                centeredSlidesBounds: true,
                rewind: true,
                watchOverflow: true,

                // Navigation arrows.
                navigation: {
                    nextEl: '.swiper-button-next-thumbnails-' + swiperId,
                    prevEl: '.swiper-button-prev-thumbnails-' + swiperId,
                },

                on: {
                    // Once slider will be iniatialized, we load all visible thumbnails.
                    imagesReady: function (s) {
                        loadSlides(s, swiperId, true);
                        var swiper = document.querySelector('.swiper-' + swiperId).swiper;
                        $('.swiper-button-next-thumbnails-' + swiperId).click(e => swiper.slideNext());
                        $('.swiper-button-prev-thumbnails-' + swiperId).click(e => swiper.slidePrev());
                    },
                    slideChange: s => loadSlides(s, swiperId),
                }
            });
        }

        var swiper = new Swiper('.swiper-' + swiperId, {
            slidesPerView: 1,
            loop: false,
            centeredSlides: true,
            watchSlidesProgress: true,

            thumbs: {
                swiper: hasThumbsSlider ? swiperThumbs : null,
            },

            // Navigation arrows.
            navigation: {
                nextEl: '.swiper-button-next-' + swiperId,
                prevEl: '.swiper-button-prev-' + swiperId,
            },

            on: {
                slideChange: function (s) {
                    if (hasThumbsSlider) {
                        loadSlides(swiperThumbs, swiperId, true)
                    }
                    else {
                        loadMediaData(s, swiperId, index);
                    }
                }
            }
        });
    }
}