function message(e) { alert(e); }
function set(key, value) { sessionStorage.setItem(key, value); }
function get(key) { return sessionStorage.getItem(key); }
function remove(key) { return sessionStorage.removeItem(key); }
function clear() { return sessionStorage.clear(); }