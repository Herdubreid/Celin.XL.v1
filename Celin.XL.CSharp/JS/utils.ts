//
export function TestStringComplete(str: string) {
    // Test Quotes
    let qoutes = (str.match(/"/g) || []).length;
    if (qoutes % 2 != 0) return false;
    // Test unmatched brackets
    var depth = 0;
    // for each char in the string : 2 cases
    for (let i = 0; i < str.length; i++) {
        if (str[i] == '[') {
            // if the char is an opening parenthesis then we increase the depth
            depth++;
        } else if (str[i] == ']') {
            // if the char is an closing parenthesis then we decrease the depth
            depth--;
        }
        //  if the depth is negative we have a closing parenthesis 
        //  before any matching opening parenthesis
        if (depth < 0) return false;
    }
    // If the depth is not null then a closing parenthesis is missing
    if (depth > 0) return false;

    // OK !
    return true;
}
