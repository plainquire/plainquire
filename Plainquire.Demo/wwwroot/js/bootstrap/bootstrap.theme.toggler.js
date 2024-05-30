/*!
 * Color mode toggler for Bootstrap's docs (https://getbootstrap.com/)
 * Copyright 2011-2024 The Bootstrap Authors
 * Licensed under the Creative Commons Attribution 3.0 Unported License.
 */

(() => {
    'use strict'

    const getStoredTheme = () => {
        return localStorage.getItem('theme');
    }

    const getPreferredTheme = () => {
        return window.matchMedia('(prefers-color-scheme: dark)').matches
            ? 'dark'
            : 'light';
    }

    const getTheme = () => {
        const storedTheme = getStoredTheme();
        if (storedTheme !== null)
            return storedTheme;

        return getPreferredTheme();
    }

    const setTheme = theme => {
        document.documentElement.setAttribute('data-bs-theme', theme);

        if (theme == getPreferredTheme())
            localStorage.removeItem('theme', theme);
        else
            localStorage.setItem('theme', theme);
    }

    setTheme(getTheme())

    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
        setTheme(getPreferredTheme())
    })

    window.setTheme = setTheme;
    window.getTheme = getTheme;
})()