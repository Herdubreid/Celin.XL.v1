export const celinql = {
    string: /"[^"]*"/,
    comment: /\/\*[^*]*\*\//,
    bold: /[fFvVwW]\w{4,7} |#\w+|@\w+|\$.+\"/,
    italic: /(?:-demo|-v2|-max)/,
    keyword: /\b(?:all|any|and|or|by|having|sum|min|max|avg|count|count_distinct|avg_distinct|group|desc|asc)\b/,
    operator: />=?|<[=>]?|\b(bw|in)\b|[=?_!^$]/,
    punctuation: /[{}[\];(),\.]/,
    number: /\b\d+\b/,
};
