// DExpressClone JS Interop Module

const scrollListeners = new Map();

/**
 * Attaches a rAF-batched passive scroll listener to the given element.
 * @param {HTMLElement} element
 * @param {object} dotnetRef - .NET object reference with InvokeMethodAsync
 */
export function attachScrollListener(element, dotnetRef) {
    if (scrollListeners.has(element)) {
        detachScrollListener(element);
    }

    let ticking = false;

    const handler = () => {
        if (!ticking) {
            ticking = true;
            requestAnimationFrame(() => {
                dotnetRef.invokeMethodAsync('OnScroll', {
                    scrollTop: element.scrollTop,
                    scrollLeft: element.scrollLeft,
                    scrollHeight: element.scrollHeight,
                    scrollWidth: element.scrollWidth,
                    clientHeight: element.clientHeight,
                    clientWidth: element.clientWidth
                });
                ticking = false;
            });
        }
    };

    element.addEventListener('scroll', handler, { passive: true });
    scrollListeners.set(element, handler);
}

/**
 * Detaches a previously attached scroll listener.
 * @param {HTMLElement} element
 */
export function detachScrollListener(element) {
    const handler = scrollListeners.get(element);
    if (handler) {
        element.removeEventListener('scroll', handler);
        scrollListeners.delete(element);
    }
}

/**
 * Returns the bounding client rect plus viewport dimensions.
 * @param {HTMLElement} element
 * @returns {{ top: number, left: number, right: number, bottom: number, width: number, height: number, viewportHeight: number, viewportWidth: number }}
 */
export function getBoundingRect(element) {
    const rect = element.getBoundingClientRect();
    return {
        top: rect.top,
        left: rect.left,
        right: rect.right,
        bottom: rect.bottom,
        width: rect.width,
        height: rect.height,
        viewportHeight: window.innerHeight,
        viewportWidth: window.innerWidth
    };
}

/**
 * Sets focus to the specified element.
 * @param {HTMLElement} element
 */
export function focusElement(element) {
    if (element) {
        element.focus();
    }
}

/**
 * Returns the width of the browser scrollbar.
 * @returns {number}
 */
// ---- Modal helpers ----

const focusTrapMap = new Map();

/**
 * Traps keyboard focus within the specified element.
 * @param {HTMLElement} element
 */
export function trapFocus(element) {
    const focusableSelector = 'a[href],button:not([disabled]),input:not([disabled]),select:not([disabled]),textarea:not([disabled]),[tabindex]:not([tabindex="-1"])';
    const handler = (e) => {
        if (e.key !== 'Tab') return;
        const focusable = [...element.querySelectorAll(focusableSelector)].filter(el => el.offsetParent !== null);
        if (focusable.length === 0) return;
        const first = focusable[0];
        const last = focusable[focusable.length - 1];
        if (e.shiftKey && document.activeElement === first) {
            e.preventDefault();
            last.focus();
        } else if (!e.shiftKey && document.activeElement === last) {
            e.preventDefault();
            first.focus();
        }
    };
    element.addEventListener('keydown', handler);
    focusTrapMap.set(element, handler);
    // Focus the first focusable element
    const first = element.querySelector(focusableSelector);
    if (first) first.focus();
}

/**
 * Releases the focus trap from the specified element.
 * @param {HTMLElement} element
 */
export function releaseFocusTrap(element) {
    const handler = focusTrapMap.get(element);
    if (handler) {
        element.removeEventListener('keydown', handler);
        focusTrapMap.delete(element);
    }
}

/**
 * Locks body scroll to prevent scrolling behind overlays.
 */
export function lockBodyScroll() {
    const scrollbarWidth = getScrollbarWidth();
    document.body.style.overflow = 'hidden';
    document.body.style.paddingRight = scrollbarWidth + 'px';
}

/**
 * Unlocks body scroll.
 */
export function unlockBodyScroll() {
    document.body.style.overflow = '';
    document.body.style.paddingRight = '';
}

export function getScrollbarWidth() {
    const outer = document.createElement('div');
    outer.style.visibility = 'hidden';
    outer.style.overflow = 'scroll';
    outer.style.msOverflowStyle = 'scrollbar';
    document.body.appendChild(outer);
    const inner = document.createElement('div');
    outer.appendChild(inner);
    const scrollbarWidth = outer.offsetWidth - inner.offsetWidth;
    outer.parentNode.removeChild(outer);
    return scrollbarWidth;
}

// ---- Dropdown positioning helpers ----

/**
 * Checks if a dropdown should open upward based on available viewport space.
 * @param {HTMLElement} triggerElement - The element that triggers the dropdown
 * @param {number} dropdownHeight - Estimated height of the dropdown in px
 * @returns {boolean} true if dropdown should open upward
 */
export function shouldDropUp(triggerElement, dropdownHeight) {
    if (!triggerElement) return false;
    const rect = triggerElement.getBoundingClientRect();
    const spaceBelow = window.innerHeight - rect.bottom;
    const spaceAbove = rect.top;
    return spaceBelow < dropdownHeight && spaceAbove > spaceBelow;
}

// ---- Click-outside helpers ----

const clickOutsideMap = new Map();

/**
 * Registers a click-outside listener for an element.
 * When a click occurs outside the element, invokes the .NET callback.
 * @param {HTMLElement} element
 * @param {object} dotnetRef
 */
export function addClickOutsideListener(element, dotnetRef) {
    removeClickOutsideListener(element);
    const handler = (e) => {
        if (element && !element.contains(e.target)) {
            dotnetRef.invokeMethodAsync('OnClickOutside');
        }
    };
    // Use setTimeout to avoid catching the current click
    setTimeout(() => document.addEventListener('mousedown', handler), 0);
    clickOutsideMap.set(element, handler);
}

/**
 * Removes a click-outside listener for an element.
 * @param {HTMLElement} element
 */
export function removeClickOutsideListener(element) {
    const handler = clickOutsideMap.get(element);
    if (handler) {
        document.removeEventListener('mousedown', handler);
        clickOutsideMap.delete(element);
    }
}
