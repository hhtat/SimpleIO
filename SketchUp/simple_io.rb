require 'sketchup.rb'
require 'extensions.rb'

simpleIOExt = SketchupExtension.new("Simple IO", "simple_io/simple_io_ext.rb");
Sketchup.register_extension(simpleIOExt, true);
