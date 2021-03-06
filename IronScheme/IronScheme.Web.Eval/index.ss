(import 
  (ironscheme)
  (web-eval)
  (only (snippets) get-snippets snippet-name snippet-id load-snippet-content)
  (ironscheme web))
  
(define (css file)
  `(link (rel . "stylesheet") (type . "text/css") (href . ,file)))
      
(define (script file)
  `(script (type . "text/javascript") (src . ,file) ""))
  
(define (is-selected? id qid)
  (and qid
      (let ((n (string->number qid)))
        (and n (fx=? id n)))))
  
(define (initial-snippet id)
  (if id
      (let ((c (load-snippet-content id)))
        (or c
            id))
      ""))

(case (http-method)
  [(post) (web-eval)]
  [(get)
    (display "<!DOCTYPE html>\n"
             (http-output-port))
    (display-html
      `(html
        (head 
          (title "IronScheme")
          (meta (name .  viewport) (content . "width=device-width, initial-scale=1, target-densitydpi=device-dpi"))
          ,(css "eval.css")
          ,(script "http://code.jquery.com/jquery-1.6.4.min.js")
          ,(script "codemirror.min.js")
          ,(css "codemirror.css")
          ,(css "default.css")
          ,(script "ga.js"))
        (body 
          (div (class . heading)
            (a (href . "http://ironscheme.codeplex.com") (target . _blank)
              (img (alt . "IronScheme") (src . "codeplex-logo.png"))))
          (form (method . post) (id . eform) (action . "index.ss")
            (textarea (rows . 8) (cols . 40) (id . expr) ,(initial-snippet (querystring 'id)))
            (div (id . commands)
              (button (id . submit) "Run") 
              (a (href . "http://ironscheme.net/doc") (target . _blank) "Documentation"))
            (div 
              (select (id . snippets) 
                      ,@(map (lambda (s)
                               `(option (value . ,(snippet-id s)) 
                                        (selected . ,(is-selected? (snippet-id s) (querystring 'id)))  
                                  ,(snippet-name s)))
                             (get-snippets)))
              (button (id . load) "Load snippet")
              (a (id . direct-link) (target . _blank) (href . "#" ) ""))
            (div 
              (input (type . text) (id . name) (value . ""))
              (button (id . save) "Save snippet"))
            (div (id . status) (h4 "Status") (pre "Ready"))
            (div (id . error) (h4 "Error") (pre ""))
            (div (id . output) (h4 "Output") (pre ""))
            (div (id . result) (h4 "Result") (pre "")))
          ,(script "eval.js"))))])
 
 #|
 Ideas:
 - integrate with documentation
 |#     