﻿(function () {
    'use strict';

    function Ctrl(element, maxCount = 9999999999) {
        var _this = this;
        this.counter = 0;
        this.maxCount = maxCount; // 保存最大數量
        this.element = element;
        this.els = {
            decrement: element.querySelector('.ctrl__button--decrement'),
            counter: {
                container: element.querySelector('.ctrl__counter'),
                num: element.querySelector('.ctrl__counter-num'),
                input: element.querySelector('.ctrl__counter-input')
            },
            increment: element.querySelector('.ctrl__button--increment')
        };

        this.decrement = function () {
            var counter = _this.getCounter();
            var nextCounter = (counter > 0) ? --counter : counter;
            _this.setCounter(nextCounter);
        };

        this.increment = function () {
            var counter = _this.getCounter();
            var nextCounter = (counter < _this.maxCount) ? ++counter : counter; // 使用 maxCount
            _this.setCounter(nextCounter);
        };

        this.getCounter = function () {
            return _this.counter;
        };

        this.setCounter = function (nextCounter) {
            _this.counter = nextCounter;
            _this.render();
        };

        this.debounce = function (callback) {
            setTimeout(callback, 100);
        };

        this.render = function (hideClassName, visibleClassName) {
            if (hideClassName && visibleClassName) {
                _this.els.counter.num.classList.add(hideClassName);
                setTimeout(function () {
                    _this.els.counter.num.innerText = _this.getCounter();
                    _this.els.counter.input.value = _this.getCounter();
                    _this.els.counter.num.classList.add(visibleClassName);
                }, 100);
                setTimeout(function () {
                    _this.els.counter.num.classList.remove(hideClassName);
                    _this.els.counter.num.classList.remove(visibleClassName);
                }, 200);
            } else {
                _this.els.counter.num.innerText = _this.getCounter();
                _this.els.counter.input.value = _this.getCounter();
            }
        };

        this.ready = function () {
            _this.els.decrement.addEventListener('click', function () {
                _this.debounce(function () {
                    _this.decrement();
                    _this.render('is-decrement-hide', 'is-decrement-visible');
                });
            });

            _this.els.increment.addEventListener('click', function () {
                _this.debounce(function () {
                    _this.increment();
                    _this.render('is-increment-hide', 'is-increment-visible');
                });
            });

            _this.els.counter.input.addEventListener('input', function (e) {
                var parseValue = parseInt(e.target.value);
                if (!isNaN(parseValue) && parseValue >= 0 && parseValue <= _this.maxCount) {
                    _this.setCounter(parseValue);
                }
            });

            _this.els.counter.input.addEventListener('focus', function (e) {
                _this.els.counter.container.classList.add('is-input');
            });

            _this.els.counter.input.addEventListener('blur', function (e) {
                _this.els.counter.container.classList.remove('is-input');
                _this.render();
            });
        };

        this.ready();
    }

    // 初始化所有控制器
    function initAllCtrls() {
        var ctrlElements = document.querySelectorAll('.ctrl');
        ctrlElements.forEach(function (element) {
            var maxCount = parseInt(element.closest('.roomA').getAttribute('data-max-count')) || 9999999999; // 從 roomA 獲取最大數量
            new Ctrl(element, maxCount); // 傳遞最大數量
        });
    }

    // 當DOM加載完成後初始化所有控制器
    document.addEventListener('DOMContentLoaded', initAllCtrls);
})();
