require 'sketchup.rb'

class SimpleIO

  TYPE_COMMENT = "#";
  TYPE_EDGE = "edge";

  HEADER_COMMENT = TYPE_COMMENT + " simple io v0";

  def initialize()
    extensionsMenu = UI.menu("Extensions");
    simpleIOMenu = extensionsMenu.add_submenu("Simple IO");
    simpleIOMenu.add_item("Import") { doImport(); }
    simpleIOMenu.add_item("Export selection") { doExport(); }
  end

  def doExport()
    begin
      _doExport();
    rescue => e
      UI.messagebox(e.inspect() + "\n" + e.backtrace.join("\n"));
    end

    if @exportFile
      @exportFile.close();
      @exportFile = nil;
    end
  end

  def doImport()
    begin
      _doImport();
    rescue => e
      UI.messagebox(e.inspect() + "\n" + e.backtrace.join("\n"));

      Sketchup.active_model.abort_operation();
    end

    if @importFile
      @importFile.close();
      @importFile = nil;
    end

    Sketchup.active_model.commit_operation();
  end

  def _doExport()
    model = Sketchup.active_model;

    if model.selection.empty?
      UI.messagebox("No selection to export!");
      return nil;
    end

    if model.path.empty?
      exportPath = UI.savepanel("Export");
    else
      exportPath = UI.savepanel("Export", File.dirname(model.path), File.basename(model.path, ".*") + ".sxf0");
    end

    return nil if not exportPath;

    @exportFile = File.new(exportPath, "w");
    @exportFile.write(HEADER_COMMENT + "\n");

    exportEntities(model.selection, Geom::Transformation.new());
  end

  def exportEntities(entities, transformation)
    for i in 0...entities.length
      exportEntity(entities[i], transformation);
    end
  end

  def exportEntity(entity, transformation)
    case entity.typename
#   when "ArcCurve"
#   when "Behavior"
#   when "ClassificationSchema"
#   when "ComponentDefinition"
    when "ComponentInstance"
      exportEntities(entity.definition.entities, transformation * entity.transformation);
    when "ConstructionLine"
    when "ConstructionPoint"
#   when "Curve"
    when "Dimension"
    when "DimensionLinear"
    when "DimensionRadial"
#   when "Drawingelement"
    when "Edge"
      exportEdge(entity, transformation);
#   when "EdgeUse"
#   when "Entity"
    when "Face"
    when "Group"
      exportEntities(entity.definition.entities, transformation * entity.transformation);
#   when "Image"
#   when "Layer"
#   when "Loop"
#   when "Material"
#   when "Page"
#   when "SectionPlane"
#   when "ShadowInfo"
#   when "Style"
#   when "Text"
#   when "Texture"
    else
      raise("Unrecognized typename: " + entity.typename);
    end
  end

  def exportEdge(edge, transformation)
    startPos = transformation * edge.start.position;
    endPos = transformation * edge.end.position;

    @exportFile.write(TYPE_EDGE + " " +
                      startPos.x.to_f().to_s() + " " +
                      startPos.y.to_f().to_s() + " " +
                      startPos.z.to_f().to_s() + " " +
                      endPos.x.to_f().to_s() + " " +
                      endPos.y.to_f().to_s() + " " +
                      endPos.z.to_f().to_s() + "\n");
  end

  def _doImport()
    importPath = UI.openpanel("Import");

    return nil if not importPath;

    @importFile = File.new(importPath, "r");
    headerLine = @importFile.gets();
    raise "Invalid header!" if not headerLine or headerLine.chomp != HEADER_COMMENT;

    model = Sketchup.active_model;
    model.start_operation("Simple IO - Import");

    @importGroup = model.active_entities.add_group();

    @importFile.each_line do |line|
      tokens = line.split();

      next if tokens.empty?;

      case tokens[0]
        when TYPE_COMMENT
        when TYPE_EDGE
          importEdge(tokens[1..-1]);
        else
          raise("Unrecognized type: " + tokens[0]);
      end
    end
  end

  def importEdge(tokens)
    startPoint = Geom::Point3d.new(tokens[0].to_f(), tokens[1].to_f(), tokens[2].to_f());
    endPoint = Geom::Point3d.new(tokens[3].to_f(), tokens[4].to_f(), tokens[5].to_f());

    @importGroup.entities.add_line(startPoint, endPoint);
  end

end

$simpleIO = SimpleIO.new();

