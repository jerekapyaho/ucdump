(ns com.coniferproductions.ucdump
  (:gen-class))

(def test-str "Na\u00EFve r\u00E9sum\u00E9s... for 0 \u20AC? Not bad!")
(def test-ch { :offset 0 :character \u20ac })
(def short-test-str "Na\u00EFve")
 
(defn character-name [x]
  (java.lang.Character/getName (int x)))
 
(defn character-line [pair]
  (let [ch (:character pair)]
    (format "%08d: U+%06X %s" (:offset pair) (int ch) (character-name ch))))
     
(defn octet-count [cp]
  "Determines the length of a Unicode codepoint when encoded in UTF-8.
  See RFC 3629 for the details."
  (cond
    (and (>= cp 0x000000) (<= cp 0x00007F)) 1
    (and (>= cp 0x000080) (<= cp 0x0007FF)) 2
    (and (>= cp 0x000800) (<= cp 0x00FFFF)) 3
    (and (>= cp 0x010000) (<= cp 0x10FFFF)) 4
    :else 0))
    
(defn octet-counts [s]
  (map octet-count (map int s)))

(defn character-lines [s]
  (let [offsets (reductions + (octet-counts s))
        pairs (map #(into {} {:offset (dec %1) :character %2}) offsets s)]
    (map character-line pairs)))
    
(defn -main
  [& args]
  (cond
    (not= (count args) 0) 
      (let [text (slurp (nth args 0) :encoding "UTF-8")]
        (doseq [line (character-lines text)] (println line)))))
    
