import 'package:flutter/foundation.dart';
import 'package:json_annotation/json_annotation.dart';

import '../base_socket_request_dto.dart';

part 'set_loop_file_request_dto.g.dart';

@JsonSerializable()
class SetLoopFileRequestDto extends BaseSocketRequestDto {
  @JsonKey(name: 'Id')
  final int id;

  @JsonKey(name: 'PlayListId')
  final int playListId;

  @JsonKey(name: 'Loop')
  final bool loop;

  SetLoopFileRequestDto({
    @required String msgType,
    @required this.id,
    @required this.playListId,
    @required this.loop,
  }) : super(messageType: msgType);

  factory SetLoopFileRequestDto.fromJson(Map<String, dynamic> json) => _$SetLoopFileRequestDtoFromJson(json);
  @override
  Map<String, dynamic> toJson() => _$SetLoopFileRequestDtoToJson(this);
}
