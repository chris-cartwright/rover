var tmp = function() {
    var locked = false;
    
    this.isLocked = function() {
        return locked;
    };
    
    this.lock = function() {
        alert("lock called");
        if(locked) {
            alert("return null");
            return null;
        }
        
        locked = true;
        return function() {
            var used = false;
            
            return function() {
                if(used) {
                    alert("already used");
                    return;
                }
                
                alert("unlocked");
                used = true;
                locked = false;
            };
        }();
    };
    
    return this;
}();

/* Progression should be:
false
lock called
true
lock called
return null
unlocked
false
already used
*/

alert(tmp.isLocked());
var unlock = tmp.lock();
alert(tmp.isLocked());
tmp.lock();
unlock();
alert(tmp.isLocked());
unlock();